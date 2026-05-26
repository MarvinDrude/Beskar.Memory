using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Beskar.Memory.Flags;

/// <summary>
/// Represents a compact structure for storing boolean values within a single byte.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct PackedBools8(byte flags) : IEquatable<PackedBools8>
{
   private byte _flags = flags;

   /// <summary>
   /// Gets the raw 8-bit byte value.
   /// </summary>
   public readonly byte RawByte
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _flags;
   }

   /// <summary>
   /// Gets or sets the boolean flag at the specified bit index.
   /// </summary>
   /// <param name="index">The 0-based bit index (0-7).</param>
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
      if ((uint)index >= 8)
      {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      if (value)
      {
         _flags |= (byte)(1 << index);
      }
      else
      {
         _flags &= (byte)~(1 << index);
      }
   }

   /// <summary>
   /// Gets the flag value at the specified index.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Get(int index)
   {
      if ((uint)index >= 8)
      {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      return (_flags & (1 << index)) != 0;
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
   public readonly bool Equals(PackedBools8 other)
   {
      return _flags == other._flags;
   }

   /// <inheritdoc />
   public readonly override bool Equals(object? obj)
   {
      return obj is PackedBools8 other && Equals(other);
   }

   /// <inheritdoc />
   public readonly override int GetHashCode()
   {
      return _flags.GetHashCode();
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(PackedBools8 left, PackedBools8 right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(PackedBools8 left, PackedBools8 right)
   {
      return !left.Equals(right);
   }
}
