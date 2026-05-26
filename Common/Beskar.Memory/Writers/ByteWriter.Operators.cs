using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Writers;

public ref partial struct ByteWriter
{
   /// <summary>
   /// Appends a single byte to the byte writer. Enables the <c>+=</c> operator.
   /// </summary>
   /// <param name="writer">The byte writer instance.</param>
   /// <param name="value">The byte value to write.</param>
   /// <returns>A reference to the modified byte writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByteWriter operator +(ByteWriter writer, byte value)
   {
      writer.WriteByte(value);
      return writer;
   }

   /// <summary>
   /// Writes a read-only span of bytes to the byte writer. Enables the <c>+=</c> operator.
   /// </summary>
   /// <param name="writer">The byte writer instance.</param>
   /// <param name="buffer">The read-only span of bytes to write.</param>
   /// <returns>A reference to the modified byte writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByteWriter operator +(ByteWriter writer, ReadOnlySpan<byte> buffer)
   {
      writer.WriteBytes(buffer);
      return writer;
   }

   /// <summary>
   /// Writes a read-only character span as UTF-8 bytes to the byte writer. Enables the <c>+=</c> operator.
   /// </summary>
   /// <param name="writer">The byte writer instance.</param>
   /// <param name="text">The string/character span to write as UTF-8.</param>
   /// <returns>A reference to the modified byte writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByteWriter operator +(ByteWriter writer, ReadOnlySpan<char> text)
   {
      writer.WriteString(text);
      return writer;
   }

   /// <summary>
   /// Appends a single byte to the byte writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   /// <param name="writer">The byte writer instance.</param>
   /// <param name="value">The byte value to write.</param>
   /// <returns>A reference to the modified byte writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByteWriter operator <<(ByteWriter writer, byte value)
   {
      writer.WriteByte(value);
      return writer;
   }

   /// <summary>
   /// Writes a read-only span of bytes to the byte writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   /// <param name="writer">The byte writer instance.</param>
   /// <param name="buffer">The read-only span of bytes to write.</param>
   /// <returns>A reference to the modified byte writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByteWriter operator <<(ByteWriter writer, ReadOnlySpan<byte> buffer)
   {
      writer.WriteBytes(buffer);
      return writer;
   }

   /// <summary>
   /// Writes a read-only character span as UTF-8 bytes to the byte writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   /// <param name="writer">The byte writer instance.</param>
   /// <param name="text">The string/character span to write as UTF-8.</param>
   /// <returns>A reference to the modified byte writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByteWriter operator <<(ByteWriter writer, ReadOnlySpan<char> text)
   {
      writer.WriteString(text);
      return writer;
   }

   /// <summary>
   /// Implicitly converts this <see cref="ByteWriter"/> to a <see cref="ReadOnlySpan{byte}"/> of the written data.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator ReadOnlySpan<byte>(ByteWriter writer) => writer.WrittenSpan;
}
