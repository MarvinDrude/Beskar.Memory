using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Beskar.Memory.Owners;
using Beskar.Memory.Utilities;

namespace Beskar.Memory.Buffers;

/// <summary>
/// A high-performance, class-based, heap-allocated circular buffer using rented memory.
/// Optimized using modulo-free arithmetic to replace slow division operations with fast branch subtractions.
/// Fully guarded against use-after-disposal bugs.
/// </summary>
public sealed class CircularBuffer<T> : IDisposable
{
   private MemoryOwner<T> _memoryOwner;
   private bool _disposed;
   private int _start;

   /// <summary>
   /// Gets the underlying span buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public Span<T> Buffer
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         return _memoryOwner.Span;
      }
   }

   /// <summary>
   /// Gets the active elements represented as a <see cref="TwoSpan{T}"/>.
   /// Handles split wrap-around cases natively and cleanly.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public TwoSpan<T> WrittenTwoSpan
   {
      get
      {
         ThrowIfDisposed();
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
            return new TwoSpan<T>(Buffer.Slice(_start, Count), []);
         }

         var firstSegment = Buffer[_start..];
         var secondSegment = Buffer[..physicalEnd];
         
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
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public ref T this[int index]
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         if ((uint)index >= (uint)Count)
         {
            throw new ArgumentOutOfRangeException(nameof(index));
         }

         var physical = _start + index;
         if (physical >= Capacity)
         {
            physical -= Capacity;
         }
         return ref Buffer[physical];
      }
   }

   /// <summary>
   /// Gets the maximum capacity of the circular buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public int Capacity
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         return _memoryOwner.Length;
      }
   }

   /// <summary>
   /// Gets the current number of elements written to the circular buffer.
   /// </summary>
   public int Count { get; private set; }

   /// <summary>
   /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class renting memory from the shared pool.
   /// </summary>
   /// <param name="capacity">The maximum capacity of the circular buffer.</param>
   public CircularBuffer(int capacity)
   {
      _memoryOwner = new MemoryOwner<T>(capacity);
      _start = 0;
      Count = 0;
   }

   /// <summary>
   /// Adds a new item to the circular buffer, overwriting the oldest item if the capacity is exceeded.
   /// </summary>
   /// <param name="item">The item to add.</param>
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public void Add(T item)
   {
      ThrowIfDisposed();
      var capacity = Capacity;
      if (capacity == 0)
      {
         return;
      }

      var index = _start + Count;
      if (index >= capacity)
      {
         index -= capacity;
      }
      
      Buffer[index] = item;

      if (Count == capacity)
      {
         _start++;
         if (_start == capacity)
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
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public bool TryDequeue([MaybeNullWhen(false)] out T item)
   {
      ThrowIfDisposed();
      if (Count == 0)
      {
         item = default;
         return false;
      }

      item = Buffer[_start];
      
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
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public T Dequeue()
   {
      ThrowIfDisposed();
      if (!TryDequeue(out var item))
      {
         throw new InvalidOperationException("Circular buffer is empty.");
      }
      return item;
   }

   /// <summary>
   /// Clears the circular buffer, resetting the logical state and clearing backing memory references.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if the circular buffer has been disposed.</exception>
   public void Clear()
   {
      ThrowIfDisposed();
      Buffer.Clear();
      Count = 0;
      _start = 0;
   }

   /// <summary>
   /// Gets a high-performance struct enumerator over the elements of the circular buffer.
   /// </summary>
   public Enumerator GetEnumerator()
   {
      ThrowIfDisposed();
      return new Enumerator(this);
   }

   /// <summary>
   /// Disposes the circular buffer, returning any rented memory to the shared pool.
   /// </summary>
   public void Dispose()
   {
      if (_disposed)
      {
         return;
      }
      _disposed = true;

      _memoryOwner.Dispose();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private void ThrowIfDisposed()
   {
      if (_disposed)
      {
         DisposedExceptionHelper.ThrowIfDisposed<CircularBuffer<T>>();
      }
   }

   /// <summary>
   /// A high-performance struct enumerator for <see cref="CircularBuffer{T}"/>.
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
      public Enumerator(CircularBuffer<T> buffer)
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
