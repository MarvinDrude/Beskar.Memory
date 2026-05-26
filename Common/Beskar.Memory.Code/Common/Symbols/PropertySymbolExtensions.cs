using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="IPropertySymbol"/> to construct property archetype models.
/// </summary>
public static class PropertySymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IPropertySymbol
   {
      /// <summary>
      /// Creates a <see cref="PropertySymbolArchetype"/> from the property symbol using the specified options.
      /// </summary>
      /// <param name="options">The options to configure archetype transformation.</param>
      /// <returns>A constructed <see cref="PropertySymbolArchetype"/> representing the property symbol.</returns>
      public PropertySymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return PropertySymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}
