using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Beskar.Memory.Timing;

/// <summary>
/// A high-performance, stack-only <see langword="ref struct"/> timer designed for zero-overhead block measurements.
/// Extremely lightweight (exactly 24 bytes in memory) using reference-based tracking.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly ref struct StackTimer
{
   private readonly long _startTicks;
   
   private readonly ref long _resultTicks;
   private readonly ref TimeSpan _resultSpan;

   /// <summary>
   /// Initializes a new instance of the <see cref="StackTimer"/> struct to output results in ticks.
   /// </summary>
   /// <param name="resultTicks">The reference to a long variable where elapsed ticks will be written.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public StackTimer(ref long resultTicks)
   {
      _resultTicks = ref resultTicks;
      _resultSpan = ref Unsafe.NullRef<TimeSpan>();
      
      _startTicks = Stopwatch.GetTimestamp();
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="StackTimer"/> struct to output results as a <see cref="TimeSpan"/>.
   /// </summary>
   /// <param name="resultSpan">The reference to a TimeSpan variable where elapsed time will be written.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public StackTimer(ref TimeSpan resultSpan)
   {
      _resultSpan = ref resultSpan;
      _resultTicks = ref Unsafe.NullRef<long>();
      
      _startTicks = Stopwatch.GetTimestamp();
   }

   /// <summary>
   /// Disposes the timer, writing elapsed ticks or time span directly to the referenced variable.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Dispose()
   {
      if (!Unsafe.IsNullRef(ref _resultSpan))
      {
         _resultSpan = Stopwatch.GetElapsedTime(_startTicks);
         return;
      }
      
      _resultTicks = Stopwatch.GetElapsedTime(_startTicks).Ticks;
   }
}
