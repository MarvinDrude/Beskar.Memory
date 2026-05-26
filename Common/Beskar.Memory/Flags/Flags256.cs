using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Flags;

/// <summary>
/// A high-performance, 256-bit bitset implemented as an inline array of four <see cref="PackedBools64"/> segments.
/// </summary>
[InlineArray(4)]
public struct Flags256 : IEquatable<Flags256>
{
   private PackedBools64 _element;

   /// <summary>
   /// Gets the boolean value of the bit at the specified index.
   /// </summary>
   /// <param name="bitIndex">The 0-based bit index (0-255).</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Get(int bitIndex)
   {
      if ((uint)bitIndex >= 256)
      {
         throw new ArgumentOutOfRangeException(nameof(bitIndex));
      }

      return this[bitIndex >> 6][bitIndex & 63];
   }

   /// <summary>
   /// Sets the boolean value of the bit at the specified index.
   /// </summary>
   /// <param name="bitIndex">The 0-based bit index (0-255).</param>
   /// <param name="value">The boolean value to set.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Set(int bitIndex, bool value)
   {
      if ((uint)bitIndex >= 256)
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
      return this[0].CountSetBits() +
             this[1].CountSetBits() +
             this[2].CountSetBits() +
             this[3].CountSetBits();
   }

   /// <summary>
   /// Sets the raw 64-bit values for all four segments.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void SetRawValues(ulong v0, ulong v1, ulong v2, ulong v3)
   {
      this[0] = new PackedBools64(v0);
      this[1] = new PackedBools64(v1);
      this[2] = new PackedBools64(v2);
      this[3] = new PackedBools64(v3);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Equals(Flags256 other)
   {
      return this[0] == other[0] &&
             this[1] == other[1] &&
             this[2] == other[2] &&
             this[3] == other[3];
   }

   /// <inheritdoc />
   public readonly override bool Equals(object? obj)
   {
      return obj is Flags256 other && Equals(other);
   }

   /// <inheritdoc />
   public readonly override int GetHashCode()
   {
      return HashCode.Combine(this[0], this[1], this[2], this[3]);
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(Flags256 left, Flags256 right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(Flags256 left, Flags256 right)
   {
      return !left.Equals(right);
   }
}
