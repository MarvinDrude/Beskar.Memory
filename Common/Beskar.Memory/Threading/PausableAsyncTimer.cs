namespace Beskar.Memory.Threading;

/// <summary>
/// A timer that can be paused and resumed, executing a callback on each tick.
/// </summary>
public sealed class PausableAsyncTimer : IDisposable
{
   private PeriodicTimer _timer;
   private TaskCompletionSource _pauseTcs;

   private readonly Func<Task> _callback;
   private readonly TimeSpan _interval;

   // State: 0 = Running, 1 = Paused
   private int _isPausedState;
   // State: 0 = Execute immediately, 1 = Delay before execution
   private int _delayNextExecution;
   // State: 0 = Active, 1 = Disposed
   private int _isDisposed;

   public PausableAsyncTimer(TimeSpan period, Func<Task> callback)
   {
      _callback = callback;
      _interval = period;

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
               _timer = new PeriodicTimer(_interval);
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

   public void Dispose()
   {
      if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
         return;

      _timer.Dispose();
      _pauseTcs.TrySetCanceled();
   }
}
