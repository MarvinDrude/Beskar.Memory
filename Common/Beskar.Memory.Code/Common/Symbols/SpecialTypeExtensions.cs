using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="SpecialType"/> to analyze primitive and numeric type categories.
/// </summary>
public static class SpecialTypeExtensions
{
   extension(SpecialType type)
   {
      /// <summary>
      /// Gets a value indicating whether the special type represents an integer-based numeric type.
      /// </summary>
      public bool IsNumber => type switch
      {
         SpecialType.System_SByte or SpecialType.System_Byte or 
            SpecialType.System_Int16 or SpecialType.System_UInt16 or
            SpecialType.System_Int32 or SpecialType.System_UInt32 or
            SpecialType.System_Int64 or SpecialType.System_UInt64 => true,
         _ => false
      };
      
      /// <summary>
      /// Gets a value indicating whether the special type represents a floating-point or decimal numeric type.
      /// </summary>
      public bool IsFloatingNumber => type switch
      {
         SpecialType.System_Single or SpecialType.System_Double or 
         SpecialType.System_Decimal => true,
         _ => false
      };
      
      /// <summary>
      /// Gets a value indicating whether the special type represents any kind of numeric type (integer, floating-point, or decimal).
      /// </summary>
      public bool IsAnyNumber => type.IsNumber || type.IsFloatingNumber;
   }
}
