using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Beskar.Memory.Buffers;

/// <summary>
/// A high-performance, stack-only <see langword="ref struct"/> representing two contiguous segments of memory.
/// Designed for wrapping circular buffer segments and split buffers cleanly.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly ref struct TwoSpan<T>
{
   private readonly Span<T> _first;
   private readonly Span<T> _second;

   /// <summary>
   /// Gets the total length of the two segments combined.
   /// </summary>
   public int Length => _first.Length + _second.Length;

   /// <summary>
   /// Gets a reference to the element at the specified index.
   /// </summary>
   /// <param name="index">The 0-based index of the element.</param>
   /// <returns>A reference to the element at the specified index.</returns>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>.</exception>
   public ref T this[int index]
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if ((uint)index >= (uint)Length)
         {
            throw new ArgumentOutOfRangeException(nameof(index));
         }

         if (index < _first.Length)
         {
            return ref _first[index];
         }
         
         return ref _second[index - _first.Length];
      }
   }

   /// <summary>
   /// Gets a reference to the element at the specified index.
   /// </summary>
   /// <param name="index">The index of the element.</param>
   /// <returns>A reference to the element at the specified index.</returns>
   public ref T this[Index index]
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref this[index.GetOffset(Length)];
   }

   /// <summary>
   /// Gets the first memory segment.
   /// </summary>
   public Span<T> First => _first;

   /// <summary>
   /// Gets the second memory segment.
   /// </summary>
   public Span<T> Second => _second;

   /// <summary>
   /// Initializes a new instance of the <see cref="TwoSpan{T}"/> struct.
   /// </summary>
   /// <param name="first">The first span segment.</param>
   /// <param name="second">The second span segment.</param>
   public TwoSpan(Span<T> first, Span<T> second)
   {
      _first = first;
      _second = second;
   }

   /// <summary>
   /// Copies the contents of this <see cref="TwoSpan{T}"/> to a destination span.
   /// </summary>
   /// <param name="destination">The destination span to copy into.</param>
   /// <exception cref="ArgumentException">Thrown if the destination span is too short.</exception>
   public void CopyTo(Span<T> destination)
   {
      if (destination.Length < Length)
      {
         throw new ArgumentException("Destination span is too short.", nameof(destination));
      }
      
      _first.CopyTo(destination);
      _second.CopyTo(destination[_first.Length..]);
   }

   /// <summary>
   /// Attempts to copy the contents of this <see cref="TwoSpan{T}"/> to a destination span.
   /// </summary>
   /// <param name="destination">The destination span to copy into.</param>
   /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
   public bool TryCopyTo(Span<T> destination)
   {
      if (destination.Length < Length)
      {
         return false;
      }
      
      _first.CopyTo(destination);
      _second.CopyTo(destination[_first.Length..]);
      return true;
   }

   /// <summary>
   /// Creates a new array and copies the elements of this <see cref="TwoSpan{T}"/> into it.
   /// </summary>
   /// <returns>A new array containing the copied elements.</returns>
   public T[] ToArray()
   {
      var array = new T[Length];
      CopyTo(array);
      return array;
   }

   /// <summary>
   /// Gets a high-performance struct enumerator over the segments.
   /// </summary>
   public Enumerator GetEnumerator() => new(_first, _second);

   /// <summary>
   /// A high-performance struct enumerator for <see cref="TwoSpan{T}"/>.
   /// </summary>
   public ref struct Enumerator
   {
      private readonly Span<T> _first;
      private readonly Span<T> _second;
      private int _index;
      private bool _inSecond;

      /// <summary>
      /// Initializes a new instance of the <see cref="Enumerator"/> struct.
      /// </summary>
      public Enumerator(Span<T> first, Span<T> second)
      {
         _first = first;
         _second = second;
         _index = -1;
         _inSecond = false;
      }

      /// <summary>
      /// Advances the enumerator to the next element.
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool MoveNext()
      {
         _index++;
         if (!_inSecond)
         {
            if (_index < _first.Length)
            {
               return true;
            }
            _index = 0;
            _inSecond = true;
         }
         return _index < _second.Length;
      }

      /// <summary>
      /// Gets a reference to the element at the current position of the enumerator.
      /// </summary>
      public ref T Current
      {
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         get => ref !_inSecond ? ref _first[_index] : ref _second[_index];
      }
   }
}
