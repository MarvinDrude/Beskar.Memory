using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Writers;

public ref partial struct BufferWriter<T>
{
   /// <summary>
   /// Appends a single element to the buffer writer. Enables the <c>+=</c> operator.
   /// </summary>
   /// <param name="writer">The buffer writer instance.</param>
   /// <param name="element">The element to add.</param>
   /// <returns>A reference to the modified buffer writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static BufferWriter<T> operator +(BufferWriter<T> writer, T element)
   {
      writer.Add(element);
      return writer;
   }

   /// <summary>
   /// Writes a read-only span of elements to the buffer writer. Enables the <c>+=</c> operator.
   /// </summary>
   /// <param name="writer">The buffer writer instance.</param>
   /// <param name="span">The read-only span to write.</param>
   /// <returns>A reference to the modified buffer writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static BufferWriter<T> operator +(BufferWriter<T> writer, ReadOnlySpan<T> span)
   {
      writer.Write(span);
      return writer;
   }

   /// <summary>
   /// Appends a single element to the buffer writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   /// <param name="writer">The buffer writer instance.</param>
   /// <param name="element">The element to add.</param>
   /// <returns>A reference to the modified buffer writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static BufferWriter<T> operator <<(BufferWriter<T> writer, T element)
   {
      writer.Add(element);
      return writer;
   }

   /// <summary>
   /// Writes a read-only span of elements to the buffer writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   /// <param name="writer">The buffer writer instance.</param>
   /// <param name="span">The read-only span to write.</param>
   /// <returns>A reference to the modified buffer writer.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static BufferWriter<T> operator <<(BufferWriter<T> writer, ReadOnlySpan<T> span)
   {
      writer.Write(span);
      return writer;
   }
}
