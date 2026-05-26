using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="ITypeParameterSymbol"/> to construct type parameter archetype models.
/// </summary>
public static class TypeParameterSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : ITypeParameterSymbol
   {
      /// <summary>
      /// Creates a <see cref="TypeParameterArchetype"/> from the type parameter symbol using the specified options.
      /// </summary>
      /// <param name="options">The options to configure archetype transformation.</param>
      /// <returns>A constructed <see cref="TypeParameterArchetype"/> representing the type parameter symbol.</returns>
      public TypeParameterArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return TypeParameterSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}
