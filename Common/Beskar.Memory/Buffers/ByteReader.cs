using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Beskar.Memory.Buffers;

/// <summary>
/// A high-performance, stack-only <see langword="ref struct"/> reader designed for zero-allocation byte and binary deserialization.
/// Supports reading from a contiguous <see cref="ReadOnlySpan{byte}"/> or a fragmented <see cref="ReadOnlySequence{byte}"/>.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{DebuggerView, nq}")]
public ref struct ByteReader
{
   private readonly ReadOnlySpan<byte> _buffer;
   private SequenceReader<byte> _sequenceReader;

   private readonly bool _isSequence;
   private int _position;

   /// <summary>
   /// Gets the number of bytes remaining to be read.
   /// </summary>
   public readonly int BytesRemaining
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _isSequence ? (int)_sequenceReader.Remaining : _buffer.Length - _position;
   }

   /// <summary>
   /// Gets or sets the current reading position.
   /// </summary>
   public int Position
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      readonly get => _isSequence ? (int)_sequenceReader.Consumed : _position;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set
      {
         if (_isSequence)
         {
            _sequenceReader = new SequenceReader<byte>(_sequenceReader.Sequence);
            _sequenceReader.Advance(value);
         }
         else
         {
            if (value < 0 || value > _buffer.Length)
            {
               throw new ArgumentOutOfRangeException(nameof(value));
            }
            _position = value;
         }
      }
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="ByteReader"/> struct reading from a contiguous span.
   /// </summary>
   /// <param name="buffer">The backing byte span.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ByteReader(ReadOnlySpan<byte> buffer)
   {
      _buffer = buffer;
      _sequenceReader = default;
      _isSequence = false;
      _position = 0;
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="ByteReader"/> struct reading from a fragmented sequence of spans.
   /// </summary>
   /// <param name="sequence">The backing read-only byte sequence.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ByteReader(ReadOnlySequence<byte> sequence)
   {
      _buffer = [];
      _sequenceReader = new SequenceReader<byte>(sequence);
      _isSequence = true;
      _position = 0;
   }

   /// <summary>
   /// Reads an unmanaged value in little-endian format from the buffer.
   /// </summary>
   /// <typeparam name="T">The type of unmanaged value to read.</typeparam>
   /// <returns>The unmanaged value read.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T ReadLittleEndian<T>()
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      if (_isSequence)
      {
         if (_sequenceReader.UnreadSpan.Length >= size)
         {
            var span = _sequenceReader.UnreadSpan[..size];
            _sequenceReader.Advance(size);
            return InterpretLittleEndian<T>(span);
         }

         Span<byte> temp = stackalloc byte[size];
         if (!_sequenceReader.TryCopyTo(temp))
         {
            throw new ArgumentOutOfRangeException(nameof(size));
         }

         _sequenceReader.Advance(size);
         return InterpretLittleEndian<T>(temp);
      }
      else
      {
         var span = _buffer.Slice(_position, size);
         _position += size;
         return InterpretLittleEndian<T>(span);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static T InterpretLittleEndian<T>(ReadOnlySpan<byte> span)
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      if (!BitConverter.IsLittleEndian)
      {
         if (size == 1)
         {
            return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(span));
         }
         else if (size == 2)
         {
            var val = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(span));
            var rev = BinaryPrimitives.ReverseEndianness(val);
            return Unsafe.As<ushort, T>(ref rev);
         }
         else if (size == 4)
         {
            var val = Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(span));
            var rev = BinaryPrimitives.ReverseEndianness(val);
            return Unsafe.As<uint, T>(ref rev);
         }
         else if (size == 8)
         {
            var val = Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(span));
            var rev = BinaryPrimitives.ReverseEndianness(val);
            return Unsafe.As<ulong, T>(ref rev);
         }
         else
         {
            Span<byte> revTemp = stackalloc byte[size];
            span.CopyTo(revTemp);
            revTemp.Reverse();
            return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(revTemp));
         }
      }
      else
      {
         return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(span));
      }
   }

   /// <summary>
   /// Reads an unmanaged value in big-endian format from the buffer.
   /// </summary>
   /// <typeparam name="T">The type of unmanaged value to read.</typeparam>
   /// <returns>The unmanaged value read.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T ReadBigEndian<T>()
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      if (_isSequence)
      {
         if (_sequenceReader.UnreadSpan.Length >= size)
         {
            var span = _sequenceReader.UnreadSpan[..size];
            _sequenceReader.Advance(size);

            return InterpretBigEndian<T>(span);
         }

         Span<byte> temp = stackalloc byte[size];
         if (!_sequenceReader.TryCopyTo(temp))
         {
            throw new ArgumentOutOfRangeException(nameof(size));
         }
         _sequenceReader.Advance(size);
         return InterpretBigEndian<T>(temp);
      }

      var spanTwo = _buffer.Slice(_position, size);
      _position += size;

      return InterpretBigEndian<T>(spanTwo);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static T InterpretBigEndian<T>(ReadOnlySpan<byte> span)
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      if (!BitConverter.IsLittleEndian)
         return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(span));

      switch (size)
      {
         case 1:
            break;
         case 2:
         {
            var val = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(span));
            var rev = BinaryPrimitives.ReverseEndianness(val);
            return Unsafe.As<ushort, T>(ref rev);
         }
         case 4:
         {
            var val = Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(span));
            var rev = BinaryPrimitives.ReverseEndianness(val);
            return Unsafe.As<uint, T>(ref rev);
         }
         case 8:
         {
            var val = Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(span));
            var rev = BinaryPrimitives.ReverseEndianness(val);
            return Unsafe.As<ulong, T>(ref rev);
         }
         default:
         {
            Span<byte> revTemp = stackalloc byte[size];
            span.CopyTo(revTemp);
            revTemp.Reverse();
            return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(revTemp));
         }
      }

      return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(span));
   }

   /// <summary>
   /// Reads a string raw without any encoding (memory recast/byte copy) to a new heap-allocated string.
   /// </summary>
   /// <param name="size">The size in bytes to read.</param>
   /// <returns>The string read.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public string ReadStringRawToString(int size)
   {
      var chars = ReadStringRaw(size);
      return new string(chars);
   }

   /// <summary>
   /// Reads a string raw without any encoding (memory recast/byte copy) directly as a character span.
   /// </summary>
   /// <param name="size">The size in bytes to read.</param>
   /// <returns>The read character span.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<char> ReadStringRaw(int size)
   {
      if (size == 0)
      {
         return [];
      }

      var raw = ReadBytes(size);
      var chars = MemoryMarshal.Cast<byte, char>(raw);

      return chars;
   }

   /// <summary>
   /// Reads a string of the specified size in bytes using custom encoding.
   /// </summary>
   /// <param name="size">The size in bytes to read.</param>
   /// <param name="encoding">The encoding to use.</param>
   /// <returns>The string read.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public string ReadString(int size, Encoding encoding)
   {
      if (_isSequence)
      {
         if (_sequenceReader.UnreadSpan.Length >= size)
         {
            var raw = _sequenceReader.UnreadSpan[..size];
            _sequenceReader.Advance(size);

            return encoding.GetString(raw);
         }

         var rented = ArrayPool<byte>.Shared.Rent(size);
         try
         {
            if (!_sequenceReader.TryCopyTo(rented.AsSpan(0, size)))
            {
               throw new ArgumentOutOfRangeException(nameof(size));
            }
            _sequenceReader.Advance(size);
            return encoding.GetString(rented, 0, size);
         }
         finally
         {
            ArrayPool<byte>.Shared.Return(rented);
         }
      }

      var result = _buffer.Slice(_position, size);
      _position += size;

      return encoding.GetString(result);
   }

   /// <summary>
   /// Reads a span of bytes of the specified length.
   /// </summary>
   /// <param name="length">The number of bytes to read.</param>
   /// <returns>The read span of bytes.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<byte> ReadBytes(int length)
   {
      if (_isSequence)
      {
         if (_sequenceReader.UnreadSpan.Length >= length)
         {
            var span = _sequenceReader.UnreadSpan[..length];
            _sequenceReader.Advance(length);
            return span;
         }

         var arr = new byte[length];
         if (!_sequenceReader.TryCopyTo(arr))
         {
            throw new ArgumentOutOfRangeException(nameof(length));
         }
         _sequenceReader.Advance(length);
         return arr;
      }

      var result = _buffer.Slice(_position, length);
      _position += length;

      return result;
   }

   /// <summary>
   /// Reads a single byte.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public byte ReadByte()
   {
      if (_isSequence)
      {
         if (!_sequenceReader.TryRead(out var value))
         {
            throw new InvalidOperationException("End of sequence reached.");
         }

         return value;
      }

      var result = _buffer[_position];
      _position++;

      return result;
   }

   private readonly string DebuggerView => _isSequence
      ? $"ByteReader[SeqConsumed={_sequenceReader.Consumed}/SeqRem={_sequenceReader.Remaining}]"
      : $"ByteReader[Pos={_position}/Cap={_buffer.Length}]";
}
