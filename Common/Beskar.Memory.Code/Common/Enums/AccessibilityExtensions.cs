using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Enums;

/// <summary>
/// Provides extension members for <see cref="Accessibility"/> to convert accessibility options to C# keywords.
/// </summary>
public static class AccessibilityExtensions
{
   extension(Accessibility access)
   {
      /// <summary>
      /// Converts the <see cref="Accessibility"/> enum value to its corresponding C# modifier keyword string.
      /// </summary>
      /// <returns>A string representing the C# accessibility modifier (e.g., "public", "private").</returns>
      public string ToKeywordString()
      {
         return access switch
         {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => "private"
         };
      }
   }
}
