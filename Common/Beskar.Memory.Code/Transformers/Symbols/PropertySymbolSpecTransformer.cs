using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

public static class PropertySymbolSpecTransformer
{
   public static PropertySymbolSpec Transform(
      IPropertySymbol propertySymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      var spec = new PropertySymbolSpec()
      {
         RefKind = propertySymbol.RefKind,
         
         IsReadOnly = propertySymbol.IsReadOnly,
         HasGetter = propertySymbol.GetMethod is not null,
         HasSetter = propertySymbol.SetMethod is not null,
         IsIndexer = propertySymbol.IsIndexer,
         IsRequired = propertySymbol.IsRequired,
      };
      
      if (options.Properties.Load.Attributes)
      {
         spec.Attributes = options.GetAttributes(propertySymbol, propertySymbol.GetAttributes());
      }

      if (depth > options.Properties.Depth)
      {
         return spec;
      }

      if (options.Properties.Load.Type)
      {
         spec.Type = TypeSymbolArchetypeTransformer.Transform(propertySymbol.Type, depth + 1, options);
      }

      if (options.Properties.Load.Getter)
      {
         spec.Getter = propertySymbol.GetMethod is not null 
            ? MethodSymbolArchetypeTransformer.Transform(propertySymbol.GetMethod, depth + 1, options)
            : null;
      }

      if (options.Properties.Load.Setter)
      {
         spec.Setter = propertySymbol.SetMethod is not null 
            ? MethodSymbolArchetypeTransformer.Transform(propertySymbol.SetMethod, depth + 1, options)
            : null;
      }
      
      return spec;
   }
}

