using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Utilities;

namespace Beskar.Memory.Owners;

/// <summary>
/// A high-performance, disposable owner of a temporary <see cref="Memory{T}"/> buffer rented from a memory pool.
/// Can also wrap an externally managed, non-pooled buffer.
/// Unlike <see cref="SpanOwner{T}"/>, this is a standard struct and can be used in asynchronous workflows, stored on the heap, or kept in class fields.
/// </summary>
/// <typeparam name="T">The type of items stored in the buffer.</typeparam>
[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{DebuggerView, nq}")]
public partial struct MemoryOwner<T> : IDisposable
{
   private ArrayPool<T>? _pool;
   private T[]? _buffer;
   private int _length;

   /// <summary>
   /// Gets the capacity of the underlying rented buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   public int Capacity
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if (_length < 0)
         {
            DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
         }
         return _buffer?.Length ?? 0;
      }
   }

   /// <summary>
   /// Gets or sets the active length of the owned buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative or exceeds the capacity.</exception>
   public int Length
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if (_length < 0)
         {
            DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
         }
         return _length;
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set
      {
         if (_length < 0)
         {
            DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
         }
         if (value < 0 || (_buffer is not null && value > _buffer.Length))
         {
            throw new ArgumentOutOfRangeException(nameof(value), "Length must be non-negative and less than or equal to Capacity.");
         }
         _length = value;
      }
   }

   /// <summary>
   /// Gets the active <see cref="Span{T}"/> representing the owned buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   public Span<T> Span
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if (_length < 0)
         {
            DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
         }
         return _buffer is null ? [] : _buffer.AsSpan(0, _length);
      }
   }

   /// <summary>
   /// Gets the active <see cref="Memory{T}"/> representing the owned buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   public Memory<T> Memory
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if (_length < 0)
         {
            DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
         }
         return _buffer?.AsMemory(0, _length) ?? default;
      }
   }

   /// <summary>
   /// Gets the underlying array buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   public T[] Buffer
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if (_length < 0)
         {
            DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
         }
         return _buffer ?? [];
      }
   }

   /// <summary>
   /// Accesses a single element within the owned buffer by index.
   /// </summary>
   /// <param name="index">The zero-based index of the element to access.</param>
   /// <returns>A mutable reference to the element at the specified index.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of bounds.</exception>
   public ref T this[int index]
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Span[index];
   }

   /// <summary>
   /// Initializes a new instance of <see cref="MemoryOwner{T}"/> wrapping an externally managed, non-pooled array.
   /// </summary>
   /// <param name="array">The array to wrap.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public MemoryOwner(T[] array)
   {
      _buffer = array;
      _length = array.Length;
      _pool = null;
   }

   /// <summary>
   /// Initializes a new instance of <see cref="MemoryOwner{T}"/> renting a buffer of a specified minimum size.
   /// </summary>
   /// <param name="minSize">The minimum number of elements required in the buffer.</param>
   /// <param name="clearArray"><see langword="true"/> to clear the rented buffer; otherwise, <see langword="false"/>.</param>
   /// <param name="pool">The custom <see cref="ArrayPool{T}"/> to rent from, or <see langword="null"/> to use the shared pool.</param>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="minSize"/> is negative.</exception>
   public MemoryOwner(
      int minSize,
      bool clearArray = true,
      ArrayPool<T>? pool = null)
   {
      switch (minSize)
      {
         case < 0:
            throw new ArgumentOutOfRangeException(nameof(minSize), "Size must be non-negative.");
         case 0:
            _buffer = null;
            _pool = null;
            _length = 0;
            return;
      }

      _pool = pool ?? ArrayPool<T>.Shared;
      _buffer = _pool.Rent(minSize);
      _length = minSize;

      if (clearArray)
      {
         _buffer.AsSpan(0, minSize).Clear();
      }
   }

   /// <summary>
   /// Attempts to resize the active length of the owned buffer.
   /// </summary>
   /// <param name="newSize">The new active length of the buffer.</param>
   /// <returns><see langword="true"/> if the length is within the capacity and was successfully resized; otherwise, <see langword="false"/>.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="newSize"/> is negative.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryResize(int newSize)
   {
      if (_length < 0)
      {
         DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
      }
      if (newSize < 0)
      {
         throw new ArgumentOutOfRangeException(nameof(newSize), "Size must be non-negative.");
      }
      if (newSize > Capacity)
      {
         return false;
      }

      _length = newSize;
      return true;
   }

   /// <summary>
   /// Exposes the underlying pooled array and its active segment.
   /// </summary>
   /// <returns>An <see cref="ArraySegment{T}"/> mapping directly to the rented buffer.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   /// <exception cref="InvalidOperationException">Thrown if this owner wraps an external non-pooled array.</exception>
   public ArraySegment<T> DangerousGetArray()
   {
      if (_length < 0)
      {
         DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
      }
      if (_buffer is null)
      {
         throw new InvalidOperationException("This MemoryOwner does not wrap a pooled array.");
      }

      return new ArraySegment<T>(_buffer, 0, _length);
   }

   /// <summary>
   /// Fills the owned buffer with a specified value.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Fill(T value) => Span.Fill(value);

   /// <summary>
   /// Clears the contents of the active buffer.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear() => Span.Clear();

   /// <summary>
   /// Copies the contents of this buffer to a destination span.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CopyTo(Span<T> destination) => Span.CopyTo(destination);

   /// <summary>
   /// Attempts to copy the contents of this buffer to a destination span.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryCopyTo(Span<T> destination) => Span.TryCopyTo(destination);

   /// <summary>
   /// Copies the contents of this buffer to a destination memory.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CopyTo(Memory<T> destination) => Memory.CopyTo(destination);

   /// <summary>
   /// Attempts to copy the contents of this buffer to a destination memory.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryCopyTo(Memory<T> destination) => Memory.TryCopyTo(destination);

   /// <summary>
   /// Slices the owned buffer.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<T> Slice(int start, int length) => Span.Slice(start, length);

   /// <summary>
   /// Get the span enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<T>.Enumerator GetEnumerator() => Span.GetEnumerator();

   /// <summary>
   /// Transfers ownership of the underlying buffer to the caller, preventing this <see cref="MemoryOwner{T}"/>
   /// from returning the buffer to the pool when disposed.
   /// </summary>
   /// <param name="length">Outputs the active length of the buffer before detachment.</param>
   /// <returns>The underlying rented array, or <see langword="null"/> if this owner did not wrap a pooled array.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   public T[]? Transfer(out int length)
   {
      if (_length < 0)
      {
         DisposedExceptionHelper.ThrowIfDisposed<MemoryOwner<T>>();
      }

      var buffer = _buffer;
      length = _length;

      _buffer = null;
      _pool = null;
      _length = -1;

      return buffer;
   }

   /// <summary>
   /// Returns the rented buffer to the pool and invalidates this owner.
   /// </summary>
   public void Dispose()
   {
      if (_length < 0)
      {
         return;
      }

      _length = -1;

      var buffer = _buffer;
      if (buffer is null || _pool is null)
      {
         return;
      }

      _buffer = null;
      _pool.Return(buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
   }

   /// <summary>
   /// Returns a string representation of this <see cref="MemoryOwner{T}"/> for debugging.
   /// </summary>
   public override string ToString()
   {
      return _length < 0
         ? "MemoryOwner(Disposed)"
         : $"MemoryOwner({(_pool is null ? "Array" : "Pooled")})[{_length}/{_buffer?.Length ?? 0}]";
   }

   internal string DebuggerView => ToString();
}
