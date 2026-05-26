using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Constants;

namespace Beskar.Memory.Extensions;

/// <summary>
/// Provides high-performance extension methods for <see cref="ReadOnlySpan{T}"/> of bytes to read unmanaged types with specific endianness.
/// </summary>
public static class SpanByteReadExtensions
{
   extension(scoped ReadOnlySpan<byte> buffer)
   {
      /// <summary>
      /// Reads an unmanaged value of type <typeparamref name="T"/> from the buffer in big-endian format.
      /// </summary>
      /// <typeparam name="T">The unmanaged type to read.</typeparam>
      /// <param name="read">When this method returns, contains the number of bytes read.</param>
      /// <returns>The unmanaged value read from the buffer.</returns>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the buffer is smaller than the size of <typeparamref name="T"/>,
      /// or if the size of <typeparamref name="T"/> exceeds the safe stack allocation limit.
      /// </exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public T ReadBigEndian<T>(out int read)
         where T : unmanaged
      {
         var size = Unsafe.SizeOf<T>();
         ArgumentOutOfRangeException.ThrowIfGreaterThan(size, BufferConstants.StackSafeByteBufferSize);

         buffer = buffer[..size];
         read = size;

         if (!BitConverter.IsLittleEndian)
         {
            return MemoryMarshal.Read<T>(buffer);
         }
         
         Span<byte> temp = stackalloc byte[size];
         for (var e = 0; e < size; e++)
         {
            temp[e] = buffer[size - 1 - e];
         }
         
         return MemoryMarshal.Read<T>(temp);
      }

      /// <summary>
      /// Reads an unmanaged value of type <typeparamref name="T"/> from the buffer in little-endian format.
      /// </summary>
      /// <typeparam name="T">The unmanaged type to read.</typeparam>
      /// <param name="read">When this method returns, contains the number of bytes read.</param>
      /// <returns>The unmanaged value read from the buffer.</returns>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the buffer is smaller than the size of <typeparamref name="T"/>,
      /// or if the size of <typeparamref name="T"/> exceeds the safe stack allocation limit.
      /// </exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public T ReadLittleEndian<T>(out int read)
         where T : unmanaged
      {
         var size = Unsafe.SizeOf<T>();
         ArgumentOutOfRangeException.ThrowIfGreaterThan(size, BufferConstants.StackSafeByteBufferSize);

         buffer = buffer[..size];
         read = size;

         if (BitConverter.IsLittleEndian)
         {
            return MemoryMarshal.Read<T>(buffer);
         }
         
         Span<byte> temp = stackalloc byte[size];
         for (var e = 0; e < size; e++)
         {
            temp[e] = buffer[size - 1 - e];
         }
         
         return MemoryMarshal.Read<T>(temp);
      }
   }
}
