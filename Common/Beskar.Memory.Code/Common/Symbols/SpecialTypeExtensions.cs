using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class SpecialTypeExtensions
{
   extension(SpecialType type)
   {
      public bool IsNumber => type switch
      {
         SpecialType.System_SByte or SpecialType.System_Byte or 
            SpecialType.System_Int16 or SpecialType.System_UInt16 or
            SpecialType.System_Int32 or SpecialType.System_UInt32 or
            SpecialType.System_Int64 or SpecialType.System_UInt64 => true,
         _ => false
      };
      
      public bool IsFloatingNumber => type switch
      {
         SpecialType.System_Single or SpecialType.System_Double or 
         SpecialType.System_Decimal => true,
         _ => false
      };
      
      public bool IsAnyNumber => type.IsNumber || type.IsFloatingNumber;
   }
}

