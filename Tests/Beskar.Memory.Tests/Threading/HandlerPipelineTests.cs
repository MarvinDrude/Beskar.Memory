using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Threading;

namespace Beskar.Memory.Tests.Threading;

public class HandlerPipelineTests
{
   public class TestContext
   {
      public int Value { get; set; }
      public ConcurrentQueue<string> Order { get; } = new();
   }

   [Fact]
   public async Task ExecutesSynchronousAndAsynchronousHandlersInOrder()
   {
      var pipeline = new HandlerPipeline<TestContext>();

      pipeline.Add(ctx =>
      {
         ctx.Order.Enqueue("sync1");
         ctx.Value += 1;
      });

      pipeline.Add(async (ctx, ct) =>
      {
         await Task.Delay(5, ct);
         ctx.Order.Enqueue("async1");
         ctx.Value += 10;
      });

      pipeline.Add((ctx, ct) =>
      {
         ctx.Order.Enqueue("sync2");
         ctx.Value += 100;
      });

      var context = new TestContext();
      await pipeline.ExecuteAsync(context, HandlerExecutionStrategy.SequentialStopOnError);

      Assert.Equal(111, context.Value);
      Assert.Equal(new[] { "sync1", "async1", "sync2" }, context.Order.ToArray());
   }

   [Fact]
   public async Task RemoveHandlerViaSubscription()
   {
      var pipeline = new HandlerPipeline<TestContext>();

      var sub1 = pipeline.Add(ctx => ctx.Order.Enqueue("one"));
      var sub2 = pipeline.Add(ctx => ctx.Order.Enqueue("two"));
      var sub3 = pipeline.Add(ctx => ctx.Order.Enqueue("three"));

      // Remove the middle one
      sub2.Dispose();

      var context = new TestContext();
      await pipeline.ExecuteAsync(context);

      Assert.Equal(new[] { "one", "three" }, context.Order.ToArray());

      // Remove the first one
      sub1.Dispose();

      context = new TestContext();
      await pipeline.ExecuteAsync(context);

      Assert.Equal(new[] { "three" }, context.Order.ToArray());
   }

   [Fact]
   public async Task SequentialStopOnErrorStrategy()
   {
      var pipeline = new HandlerPipeline<TestContext>();

      pipeline.Add(ctx => ctx.Order.Enqueue("first"));
      pipeline.Add(ctx => throw new InvalidOperationException("Fail middle"));
      pipeline.Add(ctx => ctx.Order.Enqueue("last"));

      var context = new TestContext();
      var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
      {
         await pipeline.ExecuteAsync(context, HandlerExecutionStrategy.SequentialStopOnError);
      });

      Assert.Equal("Fail middle", exception.Message);
      Assert.Equal(new[] { "first" }, context.Order.ToArray());
   }

   [Fact]
   public async Task SequentialContinueOnErrorStrategy()
   {
      var pipeline = new HandlerPipeline<TestContext>();

      pipeline.Add(ctx => ctx.Order.Enqueue("first"));
      pipeline.Add(ctx => throw new ArgumentException("Fail 1"));
      pipeline.Add(async (ctx, ct) =>
      {
         await Task.Delay(5, ct);
         ctx.Order.Enqueue("second");
      });
      pipeline.Add(ctx => throw new InvalidOperationException("Fail 2"));
      pipeline.Add(ctx => ctx.Order.Enqueue("third"));

      var context = new TestContext();
      var aggException = await Assert.ThrowsAsync<AggregateException>(async () =>
      {
         await pipeline.ExecuteAsync(context, HandlerExecutionStrategy.SequentialContinueOnError);
      });

      Assert.Equal(2, aggException.InnerExceptions.Count);
      Assert.Contains(aggException.InnerExceptions, e => e is ArgumentException && e.Message == "Fail 1");
      Assert.Contains(aggException.InnerExceptions, e => e is InvalidOperationException && e.Message == "Fail 2");

      Assert.Equal(new[] { "first", "second", "third" }, context.Order.ToArray());
   }

   [Fact]
   public async Task ParallelExecutionStrategy()
   {
      var pipeline = new HandlerPipeline<TestContext>();
      var tcs1 = new TaskCompletionSource();
      var tcs2 = new TaskCompletionSource();

      pipeline.Add(async (ctx, ct) =>
      {
         await tcs1.Task;
         ctx.Order.Enqueue("first");
      });

      pipeline.Add(async (ctx, ct) =>
      {
         await tcs2.Task;
         ctx.Order.Enqueue("second");
      });

      var context = new TestContext();
      var executeTask = pipeline.ExecuteAsync(context, HandlerExecutionStrategy.Parallel);

      // Complete in reverse registration order to prove concurrency
      tcs2.SetResult();
      tcs1.SetResult();

      await executeTask;

      // Because tcs2 completed first, "second" should be enqueued first
      Assert.Equal(new[] { "second", "first" }, context.Order.ToArray());
   }

   [Fact]
   public async Task ParallelExecutionGathersAllExceptions()
   {
      var pipeline = new HandlerPipeline<TestContext>();

      pipeline.Add(ctx => throw new ArgumentException("Parallel Fail 1"));
      pipeline.Add(async (ctx, ct) =>
      {
         await Task.Delay(5, ct);
         throw new InvalidOperationException("Parallel Fail 2");
      });
      pipeline.Add(ctx => ctx.Order.Enqueue("success"));

      var context = new TestContext();
      var aggException = await Assert.ThrowsAsync<AggregateException>(async () =>
      {
         await pipeline.ExecuteAsync(context, HandlerExecutionStrategy.Parallel);
      });

      Assert.Equal(2, aggException.InnerExceptions.Count);
      Assert.Contains(aggException.InnerExceptions, e => e is ArgumentException && e.Message == "Parallel Fail 1");
      Assert.Contains(aggException.InnerExceptions, e => e is InvalidOperationException && e.Message == "Parallel Fail 2");
      Assert.Equal(new[] { "success" }, context.Order.ToArray());
   }

   [Fact]
   public async Task CancellationHaltsExecution()
   {
      var pipeline = new HandlerPipeline<TestContext>();
      using var cts = new CancellationTokenSource();

      pipeline.Add(ctx =>
      {
         ctx.Order.Enqueue("one");
         cts.Cancel(); // Cancel execution for the next handler
      });

      pipeline.Add(ctx => ctx.Order.Enqueue("two"));

      var context = new TestContext();
      await Assert.ThrowsAsync<OperationCanceledException>(async () =>
      {
         await pipeline.ExecuteAsync(context, HandlerExecutionStrategy.SequentialStopOnError, cts.Token);
      });

      Assert.Equal(new[] { "one" }, context.Order.ToArray());
   }

   [Fact]
   public async Task ThreadSafeDynamicModificationsAndExecution()
   {
      var pipeline = new HandlerPipeline<TestContext>();
      var running = true;
      var context = new TestContext();

      // Read loop running continuously
      var readTask = Task.Run(async () =>
      {
         while (Volatile.Read(ref running))
         {
            await pipeline.ExecuteAsync(context, HandlerExecutionStrategy.Parallel);
         }
      });

      // Write loop running concurrently
      var writeTask = Task.Run(() =>
      {
         var subscriptions = new List<IDisposable>();
         for (var i = 0; i < 100; i++)
         {
            var sub = pipeline.Add(ctx => { /* noop */ });
            subscriptions.Add(sub);

            // Randomly dispose some subscriptions to stress the removal
            if (i % 2 == 0)
            {
               sub.Dispose();
            }
         }

         foreach (var sub in subscriptions)
         {
            sub.Dispose();
         }
      });

      await writeTask;
      Volatile.Write(ref running, false);
      await readTask;
   }

   [Fact]
   public void CountReflectsRegisteredHandlers()
   {
      var pipeline = new HandlerPipeline<TestContext>();
      Assert.Equal(0, pipeline.Count);

      var sub1 = pipeline.Add(ctx => { });
      Assert.Equal(1, pipeline.Count);

      var sub2 = pipeline.Add(ctx => { });
      Assert.Equal(2, pipeline.Count);

      sub1.Dispose();
      Assert.Equal(1, pipeline.Count);

      sub2.Dispose();
      Assert.Equal(0, pipeline.Count);
   }
}
