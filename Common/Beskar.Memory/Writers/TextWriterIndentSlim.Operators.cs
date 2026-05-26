using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Writers;

public ref partial struct TextWriterIndentSlim
{
   /// <summary>
   /// Appends a single character to the writer. Enables the <c>+=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator +(TextWriterIndentSlim writer, char element)
   {
      writer._buffer.Add(element);
      return writer;
   }
   
   /// <summary>
   /// Writes a read-only span of characters to the writer. Enables the <c>+=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator +(TextWriterIndentSlim writer, ReadOnlySpan<char> span)
   {
      writer.Write(span);
      return writer;
   }

   /// <summary>
   /// Writes a string to the writer. Enables the <c>+=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator +(TextWriterIndentSlim writer, string text)
   {
      writer.Write(text.AsSpan());
      return writer;
   }

   /// <summary>
   /// Appends a single character to the writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator <<(TextWriterIndentSlim writer, char element)
   {
      writer._buffer.Add(element);
      return writer;
   }
   
   /// <summary>
   /// Writes a read-only span of characters to the writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator <<(TextWriterIndentSlim writer, ReadOnlySpan<char> span)
   {
      writer.Write(span);
      return writer;
   }

   /// <summary>
   /// Writes a string to the writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator <<(TextWriterIndentSlim writer, string text)
   {
      writer.Write(text.AsSpan());
      return writer;
   }
   
   /// <summary>
   /// Writes a read-only span of characters followed by a new line. Enables the <c>|=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator |(TextWriterIndentSlim writer, ReadOnlySpan<char> span)
   {
      writer.WriteLine(span);
      return writer;
   }

   /// <summary>
   /// Writes a string followed by a new line. Enables the <c>|=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TextWriterIndentSlim operator |(TextWriterIndentSlim writer, string text)
   {
      writer.WriteLine(text.AsSpan());
      return writer;
   }
}
