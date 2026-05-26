using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Flags;
using Beskar.Memory.Owners;

namespace Beskar.Memory.Code.Common.System;

/// <summary>
/// Provides extension members for <see cref="string"/> to perform common casing and formatting operations.
/// </summary>
public static class StringExtensions
{
   extension(string str)
   {
      /// <summary>
      /// Converts the first character of the string to lowercase.
      /// </summary>
      /// <returns>The modified string with its first character lowercased.</returns>
      public string FirstCharToLower()
      {
         if (str.Length == 0 || char.IsLower(str[0])) 
            return str;
         
         return char.ToLowerInvariant(str[0]) + str[1..];
      }
      
      /// <summary>
      /// Converts the first character of the string to uppercase.
      /// </summary>
      /// <returns>The modified string with its first character uppercased.</returns>
      public string FirstCharToUpper()
      {
         if (str.Length == 0 || char.IsUpper(str[0])) 
            return str;
         
         return char.ToUpperInvariant(str[0]) + str[1..];
      }

      /// <summary>
      /// Converts the string to snake_case format.
      /// </summary>
      /// <returns>The snake_case formatted string.</returns>
      public string SnakeCase()
      {
         if (string.IsNullOrEmpty(str)) return str;

         var position = 0;
         var maxLength = str.Length * 2;
         
         using var spanOwner = maxLength < 512
            ? new SpanOwner<char>(stackalloc char[maxLength])
            : new SpanOwner<char>(maxLength);

         for (var e = 0; e < str.Length; e++)
         {
            var cha = str[e];

            if (e > 0 && char.IsUpper(cha))
            {
               spanOwner.Span[position++] = '_';
            }
            
            spanOwner.Span[position++] = char.ToLowerInvariant(cha);
         }

         return new string(spanOwner.Span[..position]);
      }
   }
}
