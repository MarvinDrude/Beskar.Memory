using System;
using System.Buffers;

namespace Beskar.Memory.Writers;

public ref partial struct TextWriterIndentSlim
{
   private static readonly SearchValues<char> HtmlSearchValues = 
      SearchValues.Create('<', '>', '&', '"', '\'');
   private static readonly SearchValues<char> HtmlSearchValuesNoApostrophe = 
      SearchValues.Create('<', '>', '&', '"');
   
   private static readonly SearchValues<char> UrlSearchValues = 
      SearchValues.Create(' ', '"', '\'', '<', '>', '(', ')', '[', ']', '\\');

   /// <summary>
   /// Writes HTML encoded text to the writer.
   /// </summary>
   public void WriteHtmlEncoded(scoped ReadOnlySpan<char> text, 
      bool multiLine = false, bool encodeApostrophe = true)
   {
      if (text.IsEmpty) return;
      
      var firstIndex = text.IndexOfAny(encodeApostrophe
         ? HtmlSearchValues : HtmlSearchValuesNoApostrophe);
      if (firstIndex == -1)
      {
         Write(text, multiLine);
         return;
      }
      
      AddIndentOnDemand();

      var lastIndex = 0;
      for (var i = 0; i < text.Length; i++)
      {
         var c = text[i];
         ReadOnlySpan<char> entity = c switch
         {
            '<' => "&lt;",
            '>' => "&gt;",
            '&' => "&amp;",
            '"' => "&quot;",
            '\'' when encodeApostrophe => "&#39;",
            _ => []
         };

         if (entity.IsEmpty) continue;
         
         // Flush preceding plain text
         if (i > lastIndex)
         {
            _buffer.Write(text[lastIndex..i]);
         }

         _buffer.Write(entity);
         lastIndex = i + 1;
      }

      if (lastIndex < text.Length)
      {
         _buffer.Write(text[lastIndex..]);
      }
   }
   
   /// <summary>
   /// Writes Markdown URL encoded text to the writer.
   /// </summary>
   public void WriteMarkdownUrlEncoded(
      scoped ReadOnlySpan<char> text, bool multiLine = false)
   {
      if (text.IsEmpty) return;
   
      var firstIndex = text.IndexOfAny(UrlSearchValues);
      if (firstIndex == -1)
      {
         Write(text, multiLine);
         return;
      }
   
      var lastIndex = 0;
   
      for (var i = firstIndex; i < text.Length; i++)
      {
         var c = text[i];
         if (c == '\\' && i + 1 < text.Length)
         {
            var next = text[i + 1];
            if (IsEscapable(next)) 
            {
               if (i > lastIndex)
               {
                  _buffer.Write(text[lastIndex..i]);
               }
            
               _buffer.Write(text.Slice(i + 1, 1));
            
               i++; 
               lastIndex = i + 1;
               
               continue;
            }
         }
         
         ReadOnlySpan<char> encoded = c switch
         {
            ' '  => "%20",
            '"'  => "%22",
            '\'' => "%27",
            '<'  => "%3C",
            '>'  => "%3E",
            '('  => "%28",
            ')'  => "%29",
            '['  => "%5B",
            ']'  => "%5D",
            '\\' => "%5C",
            _    => []
         };

         if (encoded.IsEmpty) continue;
      
         // Flush preceding plain text
         if (i > lastIndex)
         {
            _buffer.Write(text[lastIndex..i]);
         }

         _buffer.Write(encoded);
         lastIndex = i + 1;
      }

      if (lastIndex < text.Length)
      {
         _buffer.Write(text[lastIndex..]);
      }
   }

   private static bool IsEscapable(char c)
   {
      return c switch
      {
         '\\' or '`' or '*' or '_' or '{' or '}' or '[' or ']' or
            '(' or ')' or '#' or '+' or '-' or '.' or '!' or '>' or
            '"' or '\'' or '$' or '%' or '&' or ',' or '/' or ':' or
            ';' or '<' or '=' or '?' or '@' or '^' or '|' or '~' => true,
         _ => false
      };
   }
}
