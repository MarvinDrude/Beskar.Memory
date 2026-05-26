using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Utilities;

namespace Beskar.Memory.Writers;

/// <summary>
/// A high-performance, heap-allocated, disposable array builder that rents buffers from an array pool.
/// Unlike ref struct writers, this class can be kept in long-lived class fields, boxed, or passed across asynchronous task boundaries.
/// </summary>
/// <typeparam name="T">The type of items to build into the array.</typeparam>
public sealed class ArrayBuilder<T> : IDisposable
{
   private static readonly T[] EmptyPlaceholder = [];

   private readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;
   private readonly int _minCapacity;
   private bool _disposed;

   private T[] _buffer;
   private int _position;

   /// <summary>
   /// Gets the number of items currently written to the builder.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this builder has already been disposed.</exception>
   public int Count
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         return _position;
      }
   }

   /// <summary>
   /// Gets a mutable span representing the already written data.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this builder has already been disposed.</exception>
   public Span<T> WrittenSpan
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         return _buffer.AsSpan(0, _position);
      }
   }

   /// <summary>
   /// Gets the current underlying rented array buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this builder has already been disposed.</exception>
   public T[] UnderlyingArray
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         return _buffer;
      }
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="ArrayBuilder{T}"/> class with the specified minimum growth capacity.
   /// </summary>
   /// <param name="minCapacity">The minimum capacity to grow by or allocate initially, defaulting to 16.</param>
   public ArrayBuilder(int minCapacity = 16)
   {
      _minCapacity = minCapacity;
      _buffer = EmptyPlaceholder;
   }

   /// <summary>
   /// Appends a single item to the builder, automatically growing the buffer if necessary.
   /// </summary>
   /// <param name="value">The item to append.</param>
   /// <exception cref="ObjectDisposedException">Thrown if this builder has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add(scoped in T value)
   {
      ThrowIfDisposed();
      if (_position == _buffer.Length)
      {
         GrowCapacity(_buffer.Length == 0 ? _minCapacity : _buffer.Length * 2);
      }

      _buffer[_position++] = value;
   }

   /// <summary>
   /// Appends a read-only span of items to the builder, automatically growing the buffer if necessary.
   /// </summary>
   /// <param name="span">The read-only span of items to append.</param>
   /// <exception cref="ObjectDisposedException">Thrown if this builder has already been disposed.</exception>
   public void Add(ReadOnlySpan<T> span)
   {
      ThrowIfDisposed();
      var requiredLength = _position + span.Length;

      if (_buffer.Length < requiredLength)
      {
         var newCapacity = Math.Max(_buffer.Length * 2, _minCapacity);
         while (newCapacity < requiredLength)
         {
            newCapacity *= 2;
         }

         GrowCapacity(newCapacity);
      }

      span.CopyTo(_buffer.AsSpan(_position));
      _position += span.Length;
   }

   /// <summary>
   /// Sets the item at the specified index within the underlying array.
   /// </summary>
   /// <param name="index">The zero-based index at which to set the item.</param>
   /// <param name="value">The item to set.</param>
   /// <exception cref="ObjectDisposedException">Thrown if this builder has already been disposed.</exception>
   /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of bounds of the current capacity.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Set(int index, scoped in T value)
   {
      ThrowIfDisposed();
      _buffer[index] = value;
   }

   /// <summary>
   /// Clears all elements from the builder, returning any rented array to the pool.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this builder has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear()
   {
      ThrowIfDisposed();
      ReturnBuffer();

      _buffer = EmptyPlaceholder;
      _position = 0;
   }

   /// <summary>
   /// Disposes the builder, returning any rented array to the pool.
   /// </summary>
   public void Dispose()
   {
      if (_disposed)
      {
         return;
      }
      _disposed = true;

      ReturnBuffer();

      _buffer = EmptyPlaceholder;
      _position = 0;
   }

   private void GrowCapacity(int requestedSize)
   {
      var newCapacity = Math.Max(requestedSize, _minCapacity);
      var newBuffer = _pool.Rent(newCapacity);

      if (_position > 0)
      {
         _buffer.AsSpan(0, _position).CopyTo(newBuffer);
      }

      ReturnBuffer();
      _buffer = newBuffer;
   }

   private void ReturnBuffer()
   {
      if (ReferenceEquals(EmptyPlaceholder, _buffer))
      {
         return;
      }

      _pool.Return(_buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
   }

   private void ThrowIfDisposed()
   {
      if (_disposed)
      {
         throw new ObjectDisposedException(nameof(ArrayBuilder<T>));
      }
   }
}
