using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

/// <summary>
/// Provides transformation methods to convert an <see cref="IPropertySymbol"/> into a <see cref="PropertySymbolSpec"/>.
/// </summary>
public static class PropertySymbolSpecTransformer
{
   /// <summary>
   /// Transforms the specified compiler property symbol into a property symbol specification.
   /// </summary>
   /// <param name="propertySymbol">The compiler property symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="PropertySymbolSpec"/>.</returns>
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
