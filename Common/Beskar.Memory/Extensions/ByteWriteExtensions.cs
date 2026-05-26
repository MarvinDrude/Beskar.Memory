using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Extensions;

/// <summary>
/// Provides high-performance extension methods for unmanaged types to be written in specific endianness to spans and buffer writers.
/// </summary>
public static class ByteWriteExtensions
{
   extension<T>(T value)
      where T : unmanaged
   {
      /// <summary>
      /// Writes the unmanaged value to the specified span in big-endian format.
      /// </summary>
      /// <param name="buffer">The destination buffer to write the value into.</param>
      /// <returns>The number of bytes written.</returns>
      /// <exception cref="ArgumentOutOfRangeException">Thrown if the destination buffer is too small to contain the value.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int WriteBigEndian(scoped Span<byte> buffer)
      {
         var size = Unsafe.SizeOf<T>();
         buffer = buffer[..size];
         
         MemoryMarshal.Write(buffer, in value);
         if (BitConverter.IsLittleEndian)
         {
            buffer.Reverse();
         }
         
         return size;
      }

      /// <summary>
      /// Writes the unmanaged value to the specified buffer writer in big-endian format.
      /// </summary>
      /// <param name="buffer">The destination buffer writer to write the value into.</param>
      /// <param name="movePosition">A value indicating whether to advance the writer's position after writing.</param>
      /// <returns>The number of bytes written.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int WriteBigEndian(ref BufferWriter<byte> buffer, bool movePosition = true)
      {
         var size = Unsafe.SizeOf<T>();
         return value.WriteBigEndian(
            buffer.AcquireSpan(size, movePosition));
      }

      /// <summary>
      /// Writes the unmanaged value to the specified span in little-endian format.
      /// </summary>
      /// <param name="buffer">The destination buffer to write the value into.</param>
      /// <returns>The number of bytes written.</returns>
      /// <exception cref="ArgumentOutOfRangeException">Thrown if the destination buffer is too small to contain the value.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int WriteLittleEndian(scoped Span<byte> buffer)
      {
         var size = Unsafe.SizeOf<T>();
         buffer = buffer[..size];
         
         MemoryMarshal.Write(buffer, in value);
         if (!BitConverter.IsLittleEndian)
         {
            buffer.Reverse();
         }

         return size;
      }
      
      /// <summary>
      /// Writes the unmanaged value to the specified buffer writer in little-endian format.
      /// </summary>
      /// <param name="buffer">The destination buffer writer to write the value into.</param>
      /// <param name="movePosition">A value indicating whether to advance the writer's position after writing.</param>
      /// <returns>The number of bytes written.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int WriteLittleEndian(ref BufferWriter<byte> buffer, bool movePosition = true)
      {
         var size = Unsafe.SizeOf<T>();
         return value.WriteLittleEndian(
            buffer.AcquireSpan(size, movePosition));
      }
   }
}
