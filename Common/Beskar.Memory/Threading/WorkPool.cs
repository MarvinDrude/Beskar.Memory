using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Beskar.Memory.Extensions;

namespace Beskar.Memory.Threading;

/// <summary>
/// A high-performance, bounded, async work pool (actor/worker queue) designed for zero-allocation task scheduling.
/// </summary>
public sealed class WorkPool : IAsyncDisposable
{
   private readonly Channel<WorkItemBase> _items;
   private readonly List<Task> _workers = [];
   private readonly CancellationTokenSource _cts = new();

   private bool _accepting = true;
   private bool _disposed = false;

   /// <summary>
   /// Initializes a new instance of the <see cref="WorkPool"/> class with specified configuration options.
   /// </summary>
   /// <param name="options">The configurations for this work pool, or null for default options.</param>
   public WorkPool(WorkPoolOptions? options)
   {
      options ??= new WorkPoolOptions();

      var channelOptions = new BoundedChannelOptions(options.Capacity)
      {
         FullMode = options.FullMode,
         SingleReader = options.SingleReader,
         SingleWriter = false,
         AllowSynchronousContinuations = false,
      };
      _items = Channel.CreateBounded<WorkItemBase>(channelOptions);

      for (var e = 0; e < options.MaxDegreeOfParallelism; e++)
      {
         var task = Task.Factory.StartNew(
            () => RunWorker(_cts.Token), _cts.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default).Unwrap();

         _workers.Add(task);
      }
   }

   private async Task RunWorker(CancellationToken ct)
   {
      try
      {
         while (await _items.Reader.WaitToReadAsync(ct))
         {
            while (_items.Reader.TryRead(out var item))
            {
               await item.Execute(_cts.Token);
            }
         }
      }
      catch (OperationCanceledException)
      {
         // Normal shutdown
      }
   }

   /// <summary>
   /// Enqueues an asynchronous, cancellable task to the pool and returns a task representing its future result.
   /// </summary>
   /// <typeparam name="T">The type of the result returned by the task.</typeparam>
   /// <param name="func">The delegate representing the task to execute.</param>
   /// <param name="ct">A token to cancel the task execution.</param>
   /// <returns>A task representing the asynchronous completion and result of the work item.</returns>
   public Task<T> Enqueue<T>(
      Func<CancellationToken, Task<T>> func,
      CancellationToken ct = default)
   {
      if (!_accepting)
      {
         throw new InvalidOperationException("WorkPool is already completed or disposed.");
      }

      var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
      var item = new WorkItem<T>(func, tcs, ct);

      var writeTask = _items.Writer.WriteAsync(item, ct);
      return !writeTask.IsCompletedSuccessfully
         ? AwaitWriteThenResult(writeTask, tcs)
         : tcs.Task;

      static async Task<T> AwaitWriteThenResult(
         ValueTask writeTask, TaskCompletionSource<T> tcs)
      {
         await writeTask;
         return await tcs.Task;
      }
   }

   /// <summary>
   /// Enqueues a synchronous function to the pool and returns a task representing its future result.
   /// </summary>
   /// <typeparam name="T">The type of the result returned by the function.</typeparam>
   /// <param name="func">The function to execute.</param>
   /// <param name="ct">A token to cancel the function execution.</param>
   /// <returns>A task representing the asynchronous completion and result of the work item.</returns>
   public Task<T> Enqueue<T>(
      Func<T> func, CancellationToken ct = default)
   {
      return Enqueue(cancel =>
      {
         if (cancel.IsCancellationRequested)
         {
            return Task.FromCanceled<T>(cancel);
         }

         var result = func();
         return Task.FromResult(result);

      }, ct);
   }

   /// <summary>
   /// Enqueues an asynchronous, cancellable task to the pool and returns a task representing its completion.
   /// </summary>
   /// <param name="func">The delegate representing the task to execute.</param>
   /// <param name="ct">A token to cancel the task execution.</param>
   /// <returns>A task representing the asynchronous completion of the work item.</returns>
   public Task Enqueue(
      Func<CancellationToken, Task> func,
      CancellationToken ct = default)
   {
      if (!_accepting)
      {
         throw new InvalidOperationException("WorkPool is already completed or disposed.");
      }

      var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
      var item = new VoidWorkItem(func, tcs, ct);

      var writeTask = _items.Writer.WriteAsync(item, ct);
      return !writeTask.IsCompletedSuccessfully
         ? AwaitWriteThenResult(writeTask, tcs)
         : tcs.Task;

      static async Task AwaitWriteThenResult(
         ValueTask writeTask, TaskCompletionSource tcs)
      {
         await writeTask;
         await tcs.Task;
      }
   }

   /// <summary>
   /// Enqueues a synchronous action to the pool and returns a task representing its completion.
   /// </summary>
   /// <param name="action">The action to execute.</param>
   /// <param name="ct">A token to cancel the action execution.</param>
   /// <returns>A task representing the asynchronous completion of the work item.</returns>
   public Task Enqueue(
      Action action, CancellationToken ct = default)
   {
      return Enqueue(cancel =>
      {
         if (cancel.IsCancellationRequested)
         {
            return Task.FromCanceled(cancel);
         }

         action();
         return Task.CompletedTask;

      }, ct);
   }


   /// <summary>
   /// Gracefully completes the work pool, waiting for all enqueued items to be processed.
   /// </summary>
   /// <returns>A task representing the completion of all workers.</returns>
   public async Task Complete()
   {
      _accepting = false;
      _items.Writer.TryComplete();

      await Task.WhenAll(_workers)
         .WithAggregateException();
   }

   /// <summary>
   /// Disposes the work pool asynchronously, cancelling all pending tasks and shutting down workers.
   /// </summary>
   public async ValueTask DisposeAsync()
   {
      if (_disposed)
      {
         return;
      }

      _disposed = true;
      _accepting = false;

      await _cts.CancelAsync();
      _items.Writer.TryComplete();

      try
      {
         await Task.WhenAll(_workers)
            .WithAggregateException();
      }
      catch (OperationCanceledException)
      {
         // Suppress operation cancellation during disposal
      }

      while (_items.Reader.TryRead(out var item))
      {
         item.Cancel(_cts.Token);
      }

      _cts.Dispose();
   }

   private abstract class WorkItemBase
   {
      public abstract Task Execute(CancellationToken poolToken);
      public abstract void Cancel(CancellationToken ct);
   }

   private sealed class WorkItem<T>(
      Func<CancellationToken, Task<T>> func,
      TaskCompletionSource<T> tcs,
      CancellationToken ct)
      : WorkItemBase
   {
      private readonly Func<CancellationToken, Task<T>> _func = func;
      private readonly TaskCompletionSource<T> _tcs = tcs;
      private readonly CancellationToken _ct = ct;

      public override async Task Execute(CancellationToken poolToken)
      {
         if (_ct.IsCancellationRequested)
         {
            _tcs.TrySetCanceled(_ct);
            return;
         }

         try
         {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(poolToken, _ct);
            var linkedToken = linkedCts.Token;

            var result = await _func(linkedToken).ConfigureAwait(false);
            _tcs.TrySetResult(result);
         }
         catch (OperationCanceledException cancelled)
         {
            _tcs.TrySetCanceled(cancelled.CancellationToken.IsCancellationRequested
               ? cancelled.CancellationToken : _ct);
         }
         catch (Exception ex)
         {
            _tcs.TrySetException(ex);
         }
      }

      public override void Cancel(CancellationToken ct)
      {
         _tcs.TrySetCanceled(ct);
      }
   }

   private sealed class VoidWorkItem(
      Func<CancellationToken, Task> func,
      TaskCompletionSource tcs,
      CancellationToken ct)
      : WorkItemBase
   {
      private readonly Func<CancellationToken, Task> _func = func;
      private readonly TaskCompletionSource _tcs = tcs;
      private readonly CancellationToken _ct = ct;

      public override async Task Execute(CancellationToken poolToken)
      {
         if (_ct.IsCancellationRequested)
         {
            _tcs.TrySetCanceled(_ct);
            return;
         }

         try
         {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(poolToken, _ct);
            var linkedToken = linkedCts.Token;

            await _func(linkedToken).ConfigureAwait(false);
            _tcs.TrySetResult();
         }
         catch (OperationCanceledException cancelled)
         {
            _tcs.TrySetCanceled(cancelled.CancellationToken.IsCancellationRequested
               ? cancelled.CancellationToken : _ct);
         }
         catch (Exception ex)
         {
            _tcs.TrySetException(ex);
         }
      }

      public override void Cancel(CancellationToken ct)
      {
         _tcs.TrySetCanceled(ct);
      }
   }
}

