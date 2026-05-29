using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Threading;

namespace Beskar.Memory.Tests.Threading;

public class PausableAsyncTimerTests
{
   [Fact]
   public async Task Callback_ShouldExecutePeriodically()
   {
      var count = 0;
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(20), () =>
      {
         Interlocked.Increment(ref count);
         return Task.CompletedTask;
      });

      var runTask = timer.StartAsync();

      // Wait until the callback has been executed at least 3 times
      while (Volatile.Read(ref count) < 3)
      {
         await Task.Delay(5);
      }

      timer.Dispose();
      await runTask;

      Assert.True(Volatile.Read(ref count) >= 3);
   }

   [Fact]
   public async Task StartAsync_ShouldExitCleanly_OnCancellation()
   {
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(20), () => Task.CompletedTask);
      using var cts = new CancellationTokenSource();

      var runTask = timer.StartAsync(cts.Token);

      // Cancel and ensure it completes without throwing unhandled exceptions
      cts.Cancel();

      var delayTask = Task.Delay(1000);
      var completedTask = await Task.WhenAny(runTask, delayTask);

      Assert.Same(runTask, completedTask);

      timer.Dispose();
   }

   [Fact]
   public async Task Pause_ShouldSuspendCallbackExecution()
   {
      var count = 0;
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(100), () =>
      {
         Interlocked.Increment(ref count);
         return Task.CompletedTask;
      });

      var runTask = timer.StartAsync();

      // Wait for at least one tick
      while (Volatile.Read(ref count) < 1)
      {
         await Task.Delay(10);
      }

      // Pause the timer
      timer.Pause();

      // Capture the count, then wait to see if it remains frozen
      var pausedCount = Volatile.Read(ref count);
      await Task.Delay(150);

      Assert.Equal(pausedCount, Volatile.Read(ref count));

      timer.Dispose();
      await runTask;
   }

   [Fact]
   public async Task Resume_ShouldContinueCallbackExecution()
   {
      var count = 0;
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(100), () =>
      {
         Interlocked.Increment(ref count);
         return Task.CompletedTask;
      });

      var runTask = timer.StartAsync();

      // Let it tick at least once
      while (Volatile.Read(ref count) < 1)
      {
         await Task.Delay(10);
      }

      // Pause the timer
      timer.Pause();
      var pausedCount = Volatile.Read(ref count);

      // Ensure it is frozen
      await Task.Delay(150);
      Assert.Equal(pausedCount, Volatile.Read(ref count));

      // Resume immediate (waitBeforeExecution: false)
      timer.Resume(waitBeforeExecution: false);

      // It should execute immediately (almost instantly)
      await Task.Delay(40);
      Assert.True(Volatile.Read(ref count) > pausedCount);

      timer.Dispose();
      await runTask;
   }

   [Fact]
   public async Task Resume_WithWaitBeforeExecution_ShouldSkipNextExecution()
   {
      var count = 0;
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(200), () =>
      {
         Interlocked.Increment(ref count);
         return Task.CompletedTask;
      });

      var runTask = timer.StartAsync();

      // Wait for the first tick (at 200ms)
      while (Volatile.Read(ref count) < 1)
      {
         await Task.Delay(10);
      }

      // Pause the timer
      timer.Pause();

      // Ensure it is completely paused and we won't see more ticks
      await Task.Delay(300);
      var currentCount = Volatile.Read(ref count);
      Assert.Equal(1, currentCount);

      // Resume with delay (waitBeforeExecution: true)
      timer.Resume(waitBeforeExecution: true);

      // Immediately after resuming with delay, count must still be currentCount (1)
      await Task.Delay(50);
      Assert.Equal(currentCount, Volatile.Read(ref count));

      // After one full period of the new timer has elapsed (200ms), it should execute
      await Task.Delay(250);
      Assert.Equal(currentCount + 1, Volatile.Read(ref count));

      timer.Dispose();
      await runTask;
   }

   [Fact]
   public async Task Dispose_ShouldStopTimerAndExitStartAsync()
   {
      var count = 0;
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(20), () =>
      {
         Interlocked.Increment(ref count);
         return Task.CompletedTask;
      });

      var runTask = timer.StartAsync();

      // Let it tick once
      while (Volatile.Read(ref count) < 1)
      {
         await Task.Delay(5);
      }

      // Dispose it
      timer.Dispose();

      // StartAsync should exit quickly
      var delayTask = Task.Delay(1000);
      var completedTask = await Task.WhenAny(runTask, delayTask);
      Assert.Same(runTask, completedTask);
   }

   [Fact]
   public async Task Dispose_WhilePaused_ShouldExitStartAsync()
   {
      var count = 0;
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(20), () =>
      {
         Interlocked.Increment(ref count);
         return Task.CompletedTask;
      });

      var runTask = timer.StartAsync();

      // Let it tick once
      while (Volatile.Read(ref count) < 1)
      {
         await Task.Delay(5);
      }

      // Pause it
      timer.Pause();
      await Task.Delay(40);

      // Dispose it while paused
      timer.Dispose();

      // StartAsync should exit quickly
      var delayTask = Task.Delay(1000);
      var completedTask = await Task.WhenAny(runTask, delayTask);
      Assert.Same(runTask, completedTask);
   }

   [Fact]
   public void Idempotency_And_ConcurrentOperations()
   {
      var timer = new PausableAsyncTimer(TimeSpan.FromMilliseconds(50), () => Task.CompletedTask);

      // Multiple consecutive Pause calls
      timer.Pause();
      timer.Pause();

      // Multiple consecutive Resume calls
      timer.Resume();
      timer.Resume();

      // Multiple consecutive Dispose calls
      timer.Dispose();
      timer.Dispose();
   }
}
