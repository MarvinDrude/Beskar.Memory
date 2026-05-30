using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Threading;

namespace Beskar.Memory.Tests.Threading;

public class WorkPoolTests
{
   [Fact]
   public async Task EnqueueSynchronousWork()
   {
      await using var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 2 });
      
      var task1 = pool.Enqueue(() => 42);
      var task2 = pool.Enqueue(() => 100);

      var res1 = await task1;
      var res2 = await task2;

      Assert.Equal(42, res1);
      Assert.Equal(100, res2);
   }

   [Fact]
   public async Task EnqueueAsynchronousWork()
   {
      await using var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 2 });
      
      var task1 = pool.Enqueue(async ct =>
      {
         await Task.Delay(10, ct);
         return "AsyncResult";
      });

      var res = await task1;
      Assert.Equal("AsyncResult", res);
   }

   [Fact]
   public async Task ExceptionHandling()
   {
      await using var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 1 });
      
      var task = pool.Enqueue<int>(() => throw new InvalidOperationException("Failure"));

      await Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
   }

   [Fact]
   public async Task CancellationSupport()
   {
      await using var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 1 });
      
      using var cts = new CancellationTokenSource();
      cts.Cancel();

      var task = pool.Enqueue(async ct =>
      {
         await Task.Delay(1000, ct);
         return 1;
      }, cts.Token);

      await Assert.ThrowsAsync<TaskCanceledException>(async () => await task);
   }

   [Fact]
   public async Task CompletesGracefully()
   {
      var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 1 });
      var counter = 0;

      var t1 = pool.Enqueue(async ct =>
      {
         await Task.Delay(50, ct);
         Interlocked.Increment(ref counter);
         return 1;
      });

      var t2 = pool.Enqueue(async ct =>
      {
         await Task.Delay(50, ct);
         Interlocked.Increment(ref counter);
         return 2;
      });

      await pool.Complete();

      Assert.Equal(2, counter);
      Assert.Equal(1, await t1);
      Assert.Equal(2, await t2);
   }

   [Fact]
   public async Task EnqueueThrowsAfterCompleteOrDispose()
   {
      var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 1 });
      
      await pool.Complete();

      try
      {
         _ = pool.Enqueue(() => 1);
         Assert.Fail("Should have thrown InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }

      var pool2 = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 1 });
      await pool2.DisposeAsync();

      try
      {
         _ = pool2.Enqueue(() => 1);
         Assert.Fail("Should have thrown InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }
   }

   [Fact]
   public async Task EnqueueNonGenericSynchronousWork()
   {
      await using var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 2 });
      var completed = 0;

      var task1 = pool.Enqueue(() => { Interlocked.Increment(ref completed); });
      var task2 = pool.Enqueue(() => { Interlocked.Increment(ref completed); });

      await Task.WhenAll(task1, task2);

      Assert.Equal(2, completed);
   }

   [Fact]
   public async Task EnqueueNonGenericAsynchronousWork()
   {
      await using var pool = new WorkPool(new WorkPoolOptions { MaxDegreeOfParallelism = 2 });
      var completed = 0;

      var task1 = pool.Enqueue(async ct =>
      {
         await Task.Delay(10, ct);
         Interlocked.Increment(ref completed);
      });

      await task1;
      Assert.Equal(1, completed);
   }
}
