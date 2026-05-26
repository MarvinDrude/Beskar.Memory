using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Flags;

/// <summary>
/// A high-performance, 128-bit bitset implemented as an inline array of two <see cref="PackedBools64"/> segments.
/// </summary>
[InlineArray(2)]
public struct Flags128 : IEquatable<Flags128>
{
   private PackedBools64 _element;

   /// <summary>
   /// Gets the boolean value of the bit at the specified index.
   /// </summary>
   /// <param name="bitIndex">The 0-based bit index (0-127).</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Get(int bitIndex)
   {
      if ((uint)bitIndex >= 128)
      {
         throw new ArgumentOutOfRangeException(nameof(bitIndex));
      }

      return this[bitIndex >> 6][bitIndex & 63];
   }

   /// <summary>
   /// Sets the boolean value of the bit at the specified index.
   /// </summary>
   /// <param name="bitIndex">The 0-based bit index (0-127).</param>
   /// <param name="value">The boolean value to set.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Set(int bitIndex, bool value)
   {
      if ((uint)bitIndex >= 128)
      {
         throw new ArgumentOutOfRangeException(nameof(bitIndex));
      }

      ref var element = ref this[bitIndex >> 6];
      element[bitIndex & 63] = value;
   }

   /// <summary>
   /// Counts the total number of set (true) bits.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int CountSetBits()
   {
      return this[0].CountSetBits() + this[1].CountSetBits();
   }

   /// <summary>
   /// Sets the raw 64-bit values for the lower and upper segments.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void SetRawValues(ulong lower, ulong upper)
   {
      this[0] = new PackedBools64(lower);
      this[1] = new PackedBools64(upper);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Equals(Flags128 other)
   {
      return this[0] == other[0] && this[1] == other[1];
   }

   /// <inheritdoc />
   public readonly override bool Equals(object? obj)
   {
      return obj is Flags128 other && Equals(other);
   }

   /// <inheritdoc />
   public readonly override int GetHashCode()
   {
      return HashCode.Combine(this[0], this[1]);
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(Flags128 left, Flags128 right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(Flags128 left, Flags128 right)
   {
      return !left.Equals(right);
   }
}
