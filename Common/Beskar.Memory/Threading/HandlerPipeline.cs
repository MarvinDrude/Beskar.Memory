using Beskar.Memory.Writers;

namespace Beskar.Memory.Threading;

/// <summary>
/// Defines the execution strategies for handlers in the pipeline.
/// </summary>
public enum HandlerExecutionStrategy
{
   /// <summary>
   /// Executes handlers one by one in the order they were registered.
   /// If a handler throws an exception, execution stops immediately and the exception is thrown.
   /// </summary>
   SequentialStopOnError,

   /// <summary>
   /// Executes handlers one by one in the order they were registered.
   /// Even if a handler throws an exception, subsequent handlers are still executed.
   /// Any exceptions thrown will be aggregated into an <see cref="AggregateException"/> at the end.
   /// </summary>
   SequentialContinueOnError,

   /// <summary>
   /// Executes all registered handlers concurrently (in parallel) and waits for all of them to complete.
   /// If any handlers throw exceptions, they are aggregated into an <see cref="AggregateException"/> at the end.
   /// </summary>
   Parallel
}

/// <summary>
/// A thread-safe, high-performance general-purpose type that handles a list of synchronous
/// and/or asynchronous handlers and executes them using configurable execution strategies.
/// </summary>
/// <typeparam name="TContext">The type of the context passed to the handlers.</typeparam>
public sealed class HandlerPipeline<TContext>
{
   private readonly Lock _lock = new();
   private volatile Func<TContext, CancellationToken, ValueTask>[] _handlers = [];

   /// <summary>
   /// Gets the number of handlers currently registered in the pipeline.
   /// </summary>
   // ReSharper disable once InconsistentlySynchronizedField
   public int Count => _handlers.Length;

   /// <summary>
   /// Registers an asynchronous handler. Returns an <see cref="IDisposable"/> to remove the registration.
   /// </summary>
   /// <param name="handler">The asynchronous handler to register.</param>
   /// <returns>An <see cref="IDisposable"/> token. Disposing this token removes the handler from the pipeline.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
   public IDisposable Add(Func<TContext, CancellationToken, ValueTask> handler)
   {
      ArgumentNullException.ThrowIfNull(handler);

      lock (_lock)
      {
         var current = _handlers;
         var next = new Func<TContext, CancellationToken, ValueTask>[current.Length + 1];

         Array.Copy(current, next, current.Length);
         next[current.Length] = handler;

         _handlers = next;
      }

      return new Subscription(this, handler);
   }

   /// <summary>
   /// Registers a synchronous handler. Returns an <see cref="IDisposable"/> to remove the registration.
   /// </summary>
   /// <param name="handler">The synchronous handler to register.</param>
   /// <returns>An <see cref="IDisposable"/> token. Disposing this token removes the handler from the pipeline.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
   public IDisposable Add(Action<TContext> handler)
   {
      ArgumentNullException.ThrowIfNull(handler);

      return Add((ctx, _) =>
      {
         handler(ctx);
         return ValueTask.CompletedTask;
      });
   }

   /// <summary>
   /// Registers a synchronous handler that accepts a cancellation token.
   /// Returns an <see cref="IDisposable"/> to remove the registration.
   /// </summary>
   /// <param name="handler">The synchronous handler to register.</param>
   /// <returns>An <see cref="IDisposable"/> token. Disposing this token removes the handler from the pipeline.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
   public IDisposable Add(Action<TContext, CancellationToken> handler)
   {
      ArgumentNullException.ThrowIfNull(handler);

      return Add((ctx, ct) =>
      {
         handler(ctx, ct);
         return ValueTask.CompletedTask;
      });
   }

   private void Remove(Func<TContext, CancellationToken, ValueTask> handler)
   {
      lock (_lock)
      {
         var current = _handlers;
         var index = Array.IndexOf(current, handler);

         if (index < 0)
         {
            return;
         }

         var next = new Func<TContext, CancellationToken, ValueTask>[current.Length - 1];

         Array.Copy(current, 0, next, 0, index);
         Array.Copy(current, index + 1, next, index, current.Length - index - 1);

         _handlers = next;
      }
   }

   /// <summary>
   /// Executes all registered handlers using the specified strategy.
   /// </summary>
   /// <param name="context">The context to pass to each handler.</param>
   /// <param name="strategy">The execution strategy to use.</param>
   /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
   /// <returns>A <see cref="ValueTask"/> representing the pipeline execution.</returns>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="strategy"/> is not a valid strategy.</exception>
   public ValueTask ExecuteAsync(
      TContext context,
      HandlerExecutionStrategy strategy = HandlerExecutionStrategy.SequentialStopOnError,
      CancellationToken cancellationToken = default)
   {
      // ReSharper disable once InconsistentlySynchronizedField
      var handlers = _handlers;
      if (handlers.Length == 0)
      {
         return ValueTask.CompletedTask;
      }

      return strategy switch
      {
         HandlerExecutionStrategy.SequentialStopOnError => ExecuteSequentialStopOnErrorAsync(handlers, context, cancellationToken),
         HandlerExecutionStrategy.SequentialContinueOnError => ExecuteSequentialContinueOnErrorAsync(handlers, context, cancellationToken),
         HandlerExecutionStrategy.Parallel => ExecuteParallelAsync(handlers, context, cancellationToken),
         _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
      };
   }

   private static ValueTask ExecuteSequentialStopOnErrorAsync(
      Func<TContext, CancellationToken, ValueTask>[] handlers, TContext context, CancellationToken cancellationToken)
   {
      for (var i = 0; i < handlers.Length; i++)
      {
         cancellationToken.ThrowIfCancellationRequested();
         var task = handlers[i](context, cancellationToken);

         if (!task.IsCompletedSuccessfully)
         {
            return AwaitRemainingSequentialStopOnErrorAsync(handlers, i, context, task, cancellationToken);
         }
      }

      return ValueTask.CompletedTask;

      static async ValueTask AwaitRemainingSequentialStopOnErrorAsync(
         Func<TContext, CancellationToken, ValueTask>[] hdls, int startIndex,
         TContext ctx, ValueTask activeTask, CancellationToken ct)
      {
         await activeTask.ConfigureAwait(false);
         for (var i = startIndex + 1; i < hdls.Length; i++)
         {
            ct.ThrowIfCancellationRequested();
            await hdls[i](ctx, ct).ConfigureAwait(false);
         }
      }
   }

   private static ValueTask ExecuteSequentialContinueOnErrorAsync(
      Func<TContext, CancellationToken, ValueTask>[] handlers, TContext context, CancellationToken cancellationToken)
   {
      List<Exception>? exceptions = null;
      for (var i = 0; i < handlers.Length; i++)
      {
         cancellationToken.ThrowIfCancellationRequested();
         try
         {
            var task = handlers[i](context, cancellationToken);
            if (!task.IsCompletedSuccessfully)
            {
               return AwaitRemainingSequentialContinueOnErrorAsync(handlers, i, context, task, exceptions, cancellationToken);
            }
         }
         catch (Exception ex)
         {
            exceptions ??= [];
            exceptions.Add(ex);
         }
      }

      return exceptions is not null
         ? throw new AggregateException(exceptions)
         : ValueTask.CompletedTask;

      static async ValueTask AwaitRemainingSequentialContinueOnErrorAsync(
         Func<TContext, CancellationToken, ValueTask>[] hdls, int startIndex,
         TContext ctx, ValueTask activeTask, List<Exception>? excs, CancellationToken ct)
      {
         try
         {
            await activeTask.ConfigureAwait(false);
         }
         catch (Exception ex)
         {
            excs ??= [];
            excs.Add(ex);
         }

         for (var i = startIndex + 1; i < hdls.Length; i++)
         {
            ct.ThrowIfCancellationRequested();
            try
            {
               await hdls[i](ctx, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
               excs ??= [];
               excs.Add(ex);
            }
         }

         if (excs is not null)
         {
            throw new AggregateException(excs);
         }
      }
   }

   private static ValueTask ExecuteParallelAsync(
      Func<TContext, CancellationToken, ValueTask>[] handlers,
      TContext context, CancellationToken cancellationToken)
   {
      var builder = new ArrayBuilder<Task>(handlers.Length);

      try
      {
         List<Exception>? exceptions = null;

         for (var i = 0; i < handlers.Length; i++)
         {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
               var task = handlers[i](context, cancellationToken);
               if (!task.IsCompletedSuccessfully)
               {
                  builder.Add(task.AsTask());
               }
            }
            catch (Exception ex)
            {
               exceptions ??= [];
               exceptions.Add(ex);
            }
         }

         if (builder.Count != 0)
         {
            var tasksToAwait = new Task[builder.Count];
            builder.WrittenSpan.CopyTo(tasksToAwait);

            return AwaitParallelTasksAsync(tasksToAwait, exceptions);
         }

         return exceptions is not null
            ? throw new AggregateException(exceptions)
            : ValueTask.CompletedTask;
      }
      finally
      {
         builder.Dispose();
      }

      static async ValueTask AwaitParallelTasksAsync(Task[] tasksArray, List<Exception>? excs)
      {
         try
         {
            await Task.WhenAll(tasksArray).ConfigureAwait(false);
         }
         catch
         {
            // Suppress direct throwing so we can gather all inner exceptions manually below
         }

         for (var i = 0; i < tasksArray.Length; i++)
         {
            var task = tasksArray[i];

            if (task is { IsFaulted: true, Exception: not null })
            {
               excs ??= [];
               excs.AddRange(task.Exception.InnerExceptions);
            }
            else if (task.IsCanceled)
            {
               excs ??= [];
               excs.Add(new TaskCanceledException(task));
            }
         }

         if (excs is not null)
         {
            throw new AggregateException(excs);
         }
      }
   }

   private sealed class Subscription(
      HandlerPipeline<TContext> pipeline,
      Func<TContext, CancellationToken, ValueTask> handler)
      : IDisposable
   {
      private readonly HandlerPipeline<TContext> _pipeline = pipeline;
      private readonly Func<TContext, CancellationToken, ValueTask> _handler = handler;

      private int _disposed;

      public void Dispose()
      {
         if (Interlocked.Exchange(ref _disposed, 1) == 0)
         {
            _pipeline.Remove(_handler);
         }
      }
   }
}
