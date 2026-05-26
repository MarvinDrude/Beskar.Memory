using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

public static class FieldSymbolSpecTransformer
{
   public static FieldSymbolSpec Transform(
      IFieldSymbol fieldSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      var spec = new FieldSymbolSpec()
      {
         RefKind = fieldSymbol.RefKind,
         
         IsReadOnly = fieldSymbol.IsReadOnly,
         IsRequired = fieldSymbol.IsRequired,
         IsVolatile = fieldSymbol.IsVolatile,
         HasConstantValue = fieldSymbol.HasConstantValue,
         IsConst = fieldSymbol.IsConst,
      };

      if (options.Fields.Load.Attributes)
      {
         spec.Attributes = options.GetAttributes(fieldSymbol, fieldSymbol.GetAttributes());
      }

      if (depth > options.Fields.Depth)
      {
         return spec;
      }

      if (options.Fields.Load.Type)
      {
         spec.Type = TypeSymbolArchetypeTransformer.Transform(
            fieldSymbol.Type, depth + 1, options);
      }
      
      return spec;
   }
}

