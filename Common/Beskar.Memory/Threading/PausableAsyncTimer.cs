namespace Beskar.Memory.Threading;

/// <summary>
/// A timer that can be paused and resumed, executing a callback on each tick.
/// </summary>
public sealed class PausableAsyncTimer : IDisposable
{
   private PeriodicTimer _timer;
   private TaskCompletionSource _pauseTcs;

   private readonly Func<Task> _callback;
   private long _intervalTicks;
   private long _tickCount;

   // State: 0 = Running, 1 = Paused
   private int _isPausedState;
   // State: 0 = Execute immediately, 1 = Delay before execution
   private int _delayNextExecution;
   // State: 0 = Active, 1 = Disposed
   private int _isDisposed;

   /// <summary>
   /// Gets the number of ticks that have elapsed since the timer was started.
   /// (Does not reset on pause.)
   /// </summary>
   public long TickCount => Interlocked.Read(ref _tickCount);

   /// <summary>
   /// Gets a value indicating whether the timer is currently paused.
   /// </summary>
   public bool IsPaused => Volatile.Read(ref _isPausedState) == 1;

   /// <summary>
   /// Gets a value indicating whether the timer is currently running.
   /// </summary>
   public bool IsRunning => Volatile.Read(ref _isPausedState) == 0;

   /// <summary>
   /// Gets a value indicating whether the timer has been disposed.
   /// </summary>
   public bool IsDisposed => Volatile.Read(ref _isDisposed) == 1;

   /// <summary>
   /// Gets the current interval of the timer.
   /// </summary>
   public TimeSpan CurrentInterval => TimeSpan.FromTicks(Volatile.Read(ref _intervalTicks));

   public PausableAsyncTimer(TimeSpan period, Func<Task> callback)
   {
      _callback = callback;
      _intervalTicks = period.Ticks;

      _timer = new PeriodicTimer(period);
      _pauseTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

      _pauseTcs.SetResult();
   }

   public async Task StartAsync(CancellationToken ct = default)
   {
      try
      {
         while (await _timer.WaitForNextTickAsync(ct).ConfigureAwait(false))
         {
            var currentTcs = Volatile.Read(ref _pauseTcs);
            var wasPaused = !currentTcs.Task.IsCompleted;

            if (wasPaused)
            {
               // Wait for the pause to be resumed
               await currentTcs.Task.ConfigureAwait(false);

               _timer.Dispose(); // prevent timer wrap around
               if (Volatile.Read(ref _isDisposed) == 1) return;
               _timer = new PeriodicTimer(TimeSpan.FromTicks(Volatile.Read(ref _intervalTicks)));
            }

            if (Interlocked.CompareExchange(ref _delayNextExecution, 0, 1) == 1)
            {
               // Delayed execution, no further action required
               continue;
            }

            if (ct.IsCancellationRequested)
            {
               // Clean abort happened, no further action required
               return;
            }

            await _callback().ConfigureAwait(false);
            Interlocked.Increment(ref _tickCount);
         }
      }
      catch (OperationCanceledException)
      {
         // Clean abort happened, no further action required
      }
   }

   /// <summary>
   /// Resumes the timer if it was paused.
   /// </summary>
   public void Resume(bool waitBeforeExecution = false)
   {
      if (Volatile.Read(ref _isDisposed) == 1) return;

      if (Interlocked.CompareExchange(ref _isPausedState, 0, 1) != 1)
         return; // already running / someone was faster

      if (waitBeforeExecution)
      {
         // Flag the loop to delay the next execution
         Interlocked.Exchange(ref _delayNextExecution, 1);
      }

      var currentTcs = Volatile.Read(ref _pauseTcs);
      currentTcs.TrySetResult();
   }

   /// <summary>
   /// Pauses the timer.
   /// </summary>
   public void Pause()
   {
      if (Volatile.Read(ref _isDisposed) == 1) return;

      if (Interlocked.CompareExchange(ref _isPausedState, 1, 0) != 0)
         return; // already paused / someone was faster

      var newTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
      Interlocked.Exchange(ref _pauseTcs, newTcs);

      // release if dispose happened right there and then
      if (Volatile.Read(ref _isDisposed) == 1)
      {
         newTcs.TrySetCanceled();
      }
   }

   /// <summary>
   /// Updates the interval of the timer.
   /// </summary>
   public void UpdateInterval(TimeSpan newInterval)
   {
      if (Volatile.Read(ref _isDisposed) == 1) return;
      Interlocked.Exchange(ref _intervalTicks, newInterval.Ticks);

      try
      {
         var currentTimer = _timer;
         currentTimer.Period = newInterval;
      }
      catch (ObjectDisposedException)
      {
         // race condition, ignore
      }
   }

   public void Dispose()
   {
      if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
         return;

      _timer.Dispose();
      _pauseTcs.TrySetCanceled();
   }
}
