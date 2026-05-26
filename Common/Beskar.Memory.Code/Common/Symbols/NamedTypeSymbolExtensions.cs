using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="INamedTypeSymbol"/> to construct named archetype models.
/// </summary>
public static class NamedTypeSymbolExtensions
{
   extension<TSymbol>(TSymbol named)
      where TSymbol : INamedTypeSymbol
   {
      /// <summary>
      /// Creates a <see cref="NamedTypeSymbolArchetype"/> from the named type symbol using the specified options.
      /// </summary>
      /// <param name="options">The options to configure archetype transformation.</param>
      /// <returns>A constructed <see cref="NamedTypeSymbolArchetype"/> representing the named type symbol.</returns>
      public NamedTypeSymbolArchetype CreateNamedArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return NamedTypeSymbolArchetypeTransformer.Transform(named, options: options);
      }
   }
}
