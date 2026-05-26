using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Beskar.Memory.Utilities;

namespace Beskar.Memory.Writers;

/// <summary>
/// A high-performance, stack-only <see langword="ref struct"/> writer designed for zero-allocation byte and binary serialization.
/// Wraps <see cref="BufferWriter{byte}"/> for ultra-fast, branchless in-memory writing.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{DebuggerView, nq}")]
public ref partial struct ByteWriter : IDisposable
{
   private BufferWriter<byte> _writer;
   private bool _isDisposed = false;

   /// <summary>
   /// Gets the total capacity of the active buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   public readonly int Capacity
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         return _writer.Capacity;
      }
   }

   /// <summary>
   /// Gets a read-only span representing the already written data.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   public readonly ReadOnlySpan<byte> WrittenSpan
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         ThrowIfDisposed();
         return _writer.WrittenSpan;
      }
   }

   /// <summary>
   /// Gets or sets the current writing position within the active buffer.
   /// </summary>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   public int Position
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      readonly get
      {
         ThrowIfDisposed();
         return _writer.Position;
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set
      {
         ThrowIfDisposed();
         _writer.Position = value;
      }
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="ByteWriter"/> struct with an initial stack-allocated or external destination buffer.
   /// </summary>
   /// <param name="buffer">The initial buffer to write into.</param>
   /// <param name="initialMinGrowCapacity">The minimum capacity to grow by when the buffer overflows, or 512 to use the default.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ByteWriter(
      Span<byte> buffer,
      int initialMinGrowCapacity = 512)
   {
      _writer = new BufferWriter<byte>(buffer, initialMinGrowCapacity);
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="ByteWriter"/> struct pre-allocated and renting from the shared array pool.
   /// </summary>
   /// <param name="minSize">The minimum initial capacity of the rented buffer.</param>
   /// <param name="initialMinGrowCapacity">The minimum capacity to grow by when the buffer overflows, or 512 to use the default.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ByteWriter(
      int minSize,
      int initialMinGrowCapacity = 512)
   {
      _writer = new BufferWriter<byte>(minSize, initialMinGrowCapacity);
   }

   /// <summary>
   /// Writes an unmanaged value in big-endian format to the buffer.
   /// </summary>
   /// <typeparam name="T">The type of unmanaged value to write.</typeparam>
   /// <param name="value">The value to write.</param>
   /// <returns>The number of bytes written.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int WriteBigEndian<T>(T value)
      where T : unmanaged
   {
      ThrowIfDisposed();
      var size = Unsafe.SizeOf<T>();
      var span = AcquireSpan(size);

      if (BitConverter.IsLittleEndian)
      {
         if (size == 1)
         {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
         }
         else if (size == 2)
         {
            var val = Unsafe.As<T, ushort>(ref value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), BinaryPrimitives.ReverseEndianness(val));
         }
         else if (size == 4)
         {
            var val = Unsafe.As<T, uint>(ref value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), BinaryPrimitives.ReverseEndianness(val));
         }
         else if (size == 8)
         {
            var val = Unsafe.As<T, ulong>(ref value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), BinaryPrimitives.ReverseEndianness(val));
         }
         else
         {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
            span.Reverse();
         }
      }
      else
      {
         Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
      }

      return size;
   }

   /// <summary>
   /// Writes an unmanaged value in little-endian format to the buffer.
   /// </summary>
   /// <typeparam name="T">The type of unmanaged value to write.</typeparam>
   /// <param name="value">The value to write.</param>
   /// <returns>The number of bytes written.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int WriteLittleEndian<T>(T value)
      where T : unmanaged
   {
      ThrowIfDisposed();
      var size = Unsafe.SizeOf<T>();
      var span = AcquireSpan(size);

      if (!BitConverter.IsLittleEndian)
      {
         if (size == 1)
         {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
         }
         else if (size == 2)
         {
            var val = Unsafe.As<T, ushort>(ref value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), BinaryPrimitives.ReverseEndianness(val));
         }
         else if (size == 4)
         {
            var val = Unsafe.As<T, uint>(ref value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), BinaryPrimitives.ReverseEndianness(val));
         }
         else if (size == 8)
         {
            var val = Unsafe.As<T, ulong>(ref value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), BinaryPrimitives.ReverseEndianness(val));
         }
         else
         {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
            span.Reverse();
         }
      }
      else
      {
         Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
      }

      return size;
   }

   /// <summary>
   /// Writes a string raw without any encoding (memory recast/byte copy).
   /// </summary>
   /// <param name="text">The read-only span of characters to write.</param>
   /// <returns>The number of bytes written.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int WriteStringRaw(scoped ReadOnlySpan<char> text)
   {
      ThrowIfDisposed();
      var rawBytes = MemoryMarshal.AsBytes(text);
      _writer.Write(rawBytes);

      return rawBytes.Length;
   }

   /// <summary>
   /// Writes a string using default UTF8 encoding to the buffer.
   /// </summary>
   /// <param name="text">The read-only span of characters to write.</param>
   /// <returns>The number of bytes written.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int WriteString(scoped ReadOnlySpan<char> text)
   {
      ThrowIfDisposed();
      return WriteString(text, Encoding.UTF8);
   }

   /// <summary>
   /// Writes a string using custom encoding to the buffer.
   /// </summary>
   /// <param name="text">The read-only span of characters to write.</param>
   /// <param name="encoding">The encoding to use.</param>
   /// <returns>The number of bytes written.</returns>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   public int WriteString(scoped ReadOnlySpan<char> text, Encoding encoding)
   {
      ThrowIfDisposed();
      var size = encoding.GetByteCount(text);
      var span = AcquireSpan(size);

      var written = encoding.GetBytes(text, span);
      ArgumentOutOfRangeException.ThrowIfNotEqual(size, written);

      return written;
   }

   /// <summary>
   /// Writes a single byte to the buffer, growing it if necessary.
   /// </summary>
   /// <param name="value">The byte to write.</param>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteByte(byte value)
   {
      ThrowIfDisposed();
      _writer.Add(value);
   }

   /// <summary>
   /// Writes a read-only span of bytes to the buffer, growing it if necessary.
   /// </summary>
   /// <param name="buffer">The read-only span of bytes to write.</param>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteBytes(ReadOnlySpan<byte> buffer)
   {
      ThrowIfDisposed();
      _writer.Write(buffer);
   }

   /// <summary>
   /// Writes a span of bytes to the buffer, growing it if necessary.
   /// </summary>
   /// <param name="buffer">The span of bytes to write.</param>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteBytes(Span<byte> buffer)
   {
      ThrowIfDisposed();
      _writer.Write(buffer);
   }

   /// <summary>
   /// Fills the entire active buffer with the specified byte value.
   /// </summary>
   /// <param name="value">The byte value to fill the buffer with.</param>
   /// <exception cref="ObjectDisposedException">Thrown if this writer has already been disposed.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Fill(byte value)
   {
      ThrowIfDisposed();
      _writer.Fill(value);
   }

   /// <summary>
   /// Disposes the writer and releases any rented pool buffers.
   /// </summary>
   public void Dispose()
   {
      if (_isDisposed)
      {
         return;
      }
      _isDisposed = true;

      _writer.Dispose();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private Span<byte> AcquireSpan(int length)
   {
      return _writer.AcquireSpan(length);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private readonly void ThrowIfDisposed()
   {
      if (_isDisposed)
      {
         DisposedExceptionHelper.ThrowIfDisposed<ByteWriter>();
      }
   }

   private readonly string DebuggerView => $"ByteWriter[Pos={_writer.Position}/Cap={_writer.Capacity}]";
}
