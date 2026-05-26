using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Beskar.Memory.Flags;

/// <summary>
/// Represents a compact structure for storing boolean values within a 32-bit unsigned integer.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct PackedBools32(uint flags) : IEquatable<PackedBools32>
{
   private uint _flags = flags;

   /// <summary>
   /// Gets the raw 32-bit value.
   /// </summary>
   public readonly uint RawValue
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _flags;
   }

   /// <summary>
   /// Gets or sets the boolean flag at the specified bit index.
   /// </summary>
   /// <param name="index">The 0-based bit index (0-31).</param>
   public bool this[int index]
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      readonly get => Get(index);
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => Set(index, value);
   }

   /// <summary>
   /// Sets the flag at the specified index to the given value.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Set(int index, bool value)
   {
      if ((uint)index >= 32)
      {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      if (value)
      {
         _flags |= 1U << index;
      }
      else
      {
         _flags &= ~(1U << index);
      }
   }

   /// <summary>
   /// Gets the flag value at the specified index.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Get(int index)
   {
      if ((uint)index >= 32)
      {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      return (_flags & (1U << index)) != 0;
   }

   /// <summary>
   /// Counts the number of set (true) bits.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int CountSetBits()
   {
      return BitOperations.PopCount(_flags);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Equals(PackedBools32 other)
   {
      return _flags == other._flags;
   }

   /// <inheritdoc />
   public readonly override bool Equals(object? obj)
   {
      return obj is PackedBools32 other && Equals(other);
   }

   /// <inheritdoc />
   public readonly override int GetHashCode()
   {
      return _flags.GetHashCode();
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(PackedBools32 left, PackedBools32 right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(PackedBools32 left, PackedBools32 right)
   {
      return !left.Equals(right);
   }
}
