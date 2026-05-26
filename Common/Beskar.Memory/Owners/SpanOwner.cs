using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Utilities;

namespace Beskar.Memory.Owners;

/// <summary>
/// A high-performance, stack-only owner of a temporary <see cref="Span{T}"/> buffer rented from a memory pool.
/// Can also contain a <see cref="Span{T}"/> owned by the caller, which will do nothing if the owner is disposed.
/// </summary>
/// <typeparam name="T">The type of items stored in the buffer.</typeparam>
/// <remarks>
/// <para>
/// This is a stack-only <see langword="ref struct"/> designed to minimize allocations and GC pressure.
/// It relies on C# duck-typed disposal semantics to return pooled arrays upon scope exit.
/// </para>
/// <para>
/// Lifetime Rule: Always use this within a <c>using</c> block or declaration. Do not expose
/// the underlying span or array once the owner has been disposed.
/// </para>
/// </remarks>
[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{DebuggerView, nq}")]
public ref partial struct SpanOwner<T> : IDisposable
{
   private ArrayPool<T>? _pool;

   private T[]? _buffer;
   private int _length;

   private Span<T> _span;

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
            DisposedExceptionHelper.ThrowIfDisposed<SpanOwner<T>>();
         return _span;
      }
   }

   /// <summary>
   /// Gets the active length of the owned buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   public int Length
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         if (_length < 0)
            DisposedExceptionHelper.ThrowIfDisposed<SpanOwner<T>>();
         return _length;
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
   /// Initializes a new instance of <see cref="SpanOwner{T}"/> wrapping an externally managed, non-pooled span.
   /// </summary>
   /// <param name="span">The span to wrap.</param>
   /// <remarks>
   /// Since this span is externally managed, calling <see cref="Dispose"/> will not return anything to a pool,
   /// but it will clear internal references and mark the owner as disposed to prevent post-disposal access.
   /// </remarks>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SpanOwner(Span<T> span)
   {
      _buffer = null;
      _length = span.Length;

      _span = span;
   }

   /// <summary>
   /// Initializes a new instance of <see cref="SpanOwner{T}"/> renting a buffer of a specified minimum size.
   /// </summary>
   /// <param name="minSize">The minimum number of elements required in the buffer.</param>
   /// <param name="clearArray"><see langword="true"/> to clear the rented buffer; otherwise, <see langword="false"/>.</param>
   /// <param name="pool">The custom <see cref="ArrayPool{T}"/> to rent from, or <see langword="null"/> to use the shared pool.</param>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="minSize"/> is negative.</exception>
   public SpanOwner(
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
            _span = [];

            _length = 0;
            return;
      }

      _pool = pool ?? ArrayPool<T>.Shared;
      _buffer = _pool.Rent(minSize);
      _span = _buffer.AsSpan(0, minSize);

      _length = minSize;

      if (clearArray)
         _span.Clear();
   }

   /// <summary>
   /// Exposes the underlying pooled array and its active segment.
   /// </summary>
   /// <returns>An <see cref="ArraySegment{T}"/> mapping directly to the rented buffer.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   /// <exception cref="InvalidOperationException">Thrown if this owner wraps an external non-pooled span.</exception>
   public ArraySegment<T> DangerousGetArray()
   {
      if (_length < 0)
      {
         DisposedExceptionHelper.ThrowIfDisposed<SpanOwner<T>>();
      }

      if (_buffer is null)
      {
         throw new InvalidOperationException("This SpanOwner does not wrap a pooled array.");
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
   /// Transfers ownership of the underlying buffer to the caller, preventing this <see cref="SpanOwner{T}"/>
   /// from returning the buffer to the pool when disposed.
   /// </summary>
   /// <param name="length">Outputs the active length of the buffer before detachment.</param>
   /// <returns>The underlying rented array, or <see langword="null"/> if this owner did not wrap a pooled array.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this owner has already been disposed.</exception>
   public T[]? Transfer(out int length)
   {
      if (_length < 0)
         DisposedExceptionHelper.ThrowIfDisposed<SpanOwner<T>>();

      var buffer = _buffer;
      length = _length;

      _buffer = null;
      _pool = null;

      _span = default;
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
      _span = [];

      var buffer = _buffer;
      if (buffer is null
          || _pool is null) return;

      _buffer = null;
      _pool.Return(buffer, clearArray:
         RuntimeHelpers.IsReferenceOrContainsReferences<T>());
   }

   /// <summary>
   /// Returns a string representation of this <see cref="SpanOwner{T}"/> for debugging.
   /// </summary>
   public override string ToString()
   {
      return _length < 0
         ? "SpanOwner(Disposed)"
         : $"SpanOwner({(_buffer is null ? "Span" : "Pooled")})[{_length}/{_buffer?.Length ?? 0}]";
   }

   internal string DebuggerView => ToString();
}
