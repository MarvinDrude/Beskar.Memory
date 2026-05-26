using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Code;

public ref partial struct CodeTextWriter
{
   /// <summary>
   /// Appends a single character to the code writer. Enables the <c>+=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator +(CodeTextWriter writer, char element)
   {
      writer._writer += element;
      return writer;
   }
   
   /// <summary>
   /// Writes a read-only span of characters to the code writer. Enables the <c>+=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator +(CodeTextWriter writer, ReadOnlySpan<char> span)
   {
      writer._writer += span;
      return writer;
   }

   /// <summary>
   /// Writes a string to the code writer. Enables the <c>+=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator +(CodeTextWriter writer, string text)
   {
      writer._writer += text;
      return writer;
   }

   /// <summary>
   /// Appends a single character to the code writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator <<(CodeTextWriter writer, char element)
   {
      writer._writer <<= element;
      return writer;
   }
   
   /// <summary>
   /// Writes a read-only span of characters to the code writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator <<(CodeTextWriter writer, ReadOnlySpan<char> span)
   {
      writer._writer <<= span;
      return writer;
   }

   /// <summary>
   /// Writes a string to the code writer. Enables the <c>&lt;&lt;=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator <<(CodeTextWriter writer, string text)
   {
      writer._writer <<= text;
      return writer;
   }
   
   /// <summary>
   /// Writes a read-only span of characters followed by a new line. Enables the <c>|=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator |(CodeTextWriter writer, ReadOnlySpan<char> span)
   {
      writer._writer |= span;
      return writer;
   }

   /// <summary>
   /// Writes a string followed by a new line. Enables the <c>|=</c> operator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CodeTextWriter operator |(CodeTextWriter writer, string text)
   {
      writer._writer |= text;
      return writer;
   }
}
