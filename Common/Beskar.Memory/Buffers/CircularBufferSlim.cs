using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Beskar.Memory.Buffers;

/// <summary>
/// A high-performance, stack-only <see langword="ref struct"/> circular buffer wrapping a contiguous span.
/// Optimized using modulo-free arithmetic to replace slow division operations with fast branch subtractions.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public ref struct CircularBufferSlim<T>
{
   private readonly Span<T> _buffer;
   private int _start;

   /// <summary>
   /// Gets the underlying span buffer.
   /// </summary>
   public Span<T> Buffer
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _buffer;
   }

   /// <summary>
   /// Gets the active elements represented as a <see cref="TwoSpan{T}"/>.
   /// Handles split wrap-around cases natively and cleanly.
   /// </summary>
   public TwoSpan<T> WrittenTwoSpan
   {
      get
      {
         if (Count == 0)
         {
            return new TwoSpan<T>([], []);
         }

         var physicalEnd = _start + Count;
         if (physicalEnd >= Capacity)
         {
            physicalEnd -= Capacity;
         }

         if (_start < physicalEnd)
         {
            return new TwoSpan<T>(_buffer.Slice(_start, Count), []);
         }

         var firstSegment = _buffer[_start..];
         var secondSegment = _buffer[..physicalEnd];
         
         return new TwoSpan<T>(firstSegment, secondSegment);
      }
   }

   /// <summary>
   /// Gets a reference to the element at the specified logical index.
   /// Indexing is relative to the start of the circular buffer.
   /// </summary>
   /// <param name="index">The logical 0-based index.</param>
   /// <returns>A reference to the element at the specified index.</returns>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is negative or greater than or equal to <see cref="Count"/>.</exception>
   public ref T this[int index]
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if ((uint)index >= (uint)Count)
         {
            throw new ArgumentOutOfRangeException(nameof(index));
         }

         var physical = _start + index;
         if (physical >= Capacity)
         {
            physical -= Capacity;
         }
         return ref _buffer[physical];
      }
   }

   /// <summary>
   /// Gets the maximum capacity of the circular buffer.
   /// </summary>
   public int Capacity => _buffer.Length;

   /// <summary>
   /// Gets the current number of elements written to the circular buffer.
   /// </summary>
   public int Count { get; private set; }

   /// <summary>
   /// Initializes a new instance of the <see cref="CircularBufferSlim{T}"/> struct.
   /// </summary>
   /// <param name="buffer">The backing span buffer to write into.</param>
   public CircularBufferSlim(Span<T> buffer)
   {
      _buffer = buffer;
      _start = 0;
      Count = 0;
   }

   /// <summary>
   /// Adds a new item to the circular buffer, overwriting the oldest item if the capacity is exceeded.
   /// </summary>
   /// <param name="item">The item to add.</param>
   public void Add(T item)
   {
      if (Capacity == 0)
      {
         return;
      }

      var index = _start + Count;
      if (index >= Capacity)
      {
         index -= Capacity;
      }
      
      _buffer[index] = item;

      if (Count == Capacity)
      {
         _start++;
         if (_start == Capacity)
         {
            _start = 0;
         }
      }
      else
      {
         Count++;
      }
   }

   /// <summary>
   /// Attempts to remove and return the oldest item from the start of the circular buffer.
   /// </summary>
   /// <param name="item">The dequeued item, or default if the buffer is empty.</param>
   /// <returns><see langword="true"/> if an item was successfully dequeued; otherwise, <see langword="false"/>.</returns>
   public bool TryDequeue([MaybeNullWhen(false)] out T item)
   {
      if (Count == 0)
      {
         item = default;
         return false;
      }

      item = _buffer[_start];
      _buffer[_start] = default!;
      
      _start++;
      if (_start == Capacity)
      {
         _start = 0;
      }
      
      Count--;
      return true;
   }

   /// <summary>
   /// Removes and returns the oldest item from the start of the circular buffer.
   /// </summary>
   /// <returns>The dequeued item.</returns>
   /// <exception cref="InvalidOperationException">Thrown if the buffer is empty.</exception>
   public T Dequeue()
   {
      if (!TryDequeue(out var item))
      {
         throw new InvalidOperationException("Circular buffer is empty.");
      }
      return item;
   }

   /// <summary>
   /// Clears the circular buffer, resetting the logical state and clearing backing memory references.
   /// </summary>
   public void Clear()
   {
      _buffer.Clear();
      Count = 0;
      _start = 0;
   }

   /// <summary>
   /// Gets a high-performance struct enumerator over the elements of the circular buffer.
   /// </summary>
   public Enumerator GetEnumerator() => new(this);

   /// <summary>
   /// A high-performance struct enumerator for <see cref="CircularBufferSlim{T}"/>.
   /// </summary>
   public ref struct Enumerator
   {
      private readonly Span<T> _buffer;
      private readonly int _start;
      private readonly int _count;
      private readonly int _capacity;
      private int _index;

      /// <summary>
      /// Initializes a new instance of the <see cref="Enumerator"/> struct.
      /// </summary>
      /// <param name="buffer">The circular buffer instance to enumerate.</param>
      public Enumerator(scoped in CircularBufferSlim<T> buffer)
      {
         _buffer = buffer.Buffer;
         _start = buffer._start;
         _count = buffer.Count;
         _capacity = buffer.Capacity;
         _index = -1;
      }

      /// <summary>
      /// Advances the enumerator to the next element.
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool MoveNext()
      {
         _index++;
         return _index < _count;
      }

      /// <summary>
      /// Gets a reference to the element at the current position of the enumerator.
      /// </summary>
      public ref T Current
      {
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         get
         {
            var physical = _start + _index;
            if (physical >= _capacity)
            {
               physical -= _capacity;
            }
            return ref _buffer[physical];
         }
      }
   }
}
