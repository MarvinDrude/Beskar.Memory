using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Timing;

/// <summary>
/// A high-performance, stack-only <see langword="ref struct"/> timer designed for asynchronous operations.
/// Writes elapsed time to an <see cref="AsyncTimerResult"/> block when disposed.
/// </summary>
public readonly struct AsyncTimer : IDisposable
{
   private readonly long _startTicks;
   private readonly AsyncTimerResult _result;

   /// <summary>
   /// Initializes a new instance of the <see cref="AsyncTimer"/> struct.
   /// </summary>
   /// <param name="result">The timer result instance to populate upon disposal.</param>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is null.</exception>
   public AsyncTimer(AsyncTimerResult result)
   {
      ArgumentNullException.ThrowIfNull(result);
      
      _result = result;
      _startTicks = Stopwatch.GetTimestamp();
   }

   /// <summary>
   /// Disposes the timer, calculating elapsed time and writing it to the result block.
   /// </summary>
   public void Dispose()
   {
      _result.Elapsed = Stopwatch.GetElapsedTime(_startTicks);
   }
}

/// <summary>
/// Represents the result block that stores the measured elapsed time of an asynchronous operation.
/// </summary>
public sealed class AsyncTimerResult
{
   /// <summary>
   /// Gets or sets the measured elapsed time span.
   /// </summary>
   public TimeSpan Elapsed { get; internal set; }

   /// <summary>
   /// Gets the elapsed time represented in milliseconds.
   /// </summary>
   public double ElapsedMilliseconds => Elapsed.TotalMilliseconds;

   /// <summary>
   /// Gets the elapsed time represented in ticks.
   /// </summary>
   public long ElapsedTicks => Elapsed.Ticks;

   /// <summary>
   /// Resets the elapsed time back to zero.
   /// </summary>
   public void Reset()
   {
      Elapsed = TimeSpan.Zero;
   }

   /// <summary>
   /// Returns a professionally formatted string representation of the elapsed time.
   /// </summary>
   public override string ToString()
   {
      return $"{ElapsedMilliseconds:F2} ms";
   }
}
