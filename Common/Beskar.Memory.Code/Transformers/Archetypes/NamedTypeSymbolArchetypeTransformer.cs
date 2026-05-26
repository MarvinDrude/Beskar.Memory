using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

/// <summary>
/// Provides transformation methods to convert an <see cref="INamedTypeSymbol"/> into a <see cref="NamedTypeSymbolArchetype"/>.
/// </summary>
public static class NamedTypeSymbolArchetypeTransformer
{
   /// <summary>
   /// Transforms the specified compiler named type symbol into a named type symbol archetype representation.
   /// </summary>
   /// <param name="namedTypeSymbol">The compiler named type symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="NamedTypeSymbolArchetype"/>.</returns>
   public static NamedTypeSymbolArchetype Transform(
      INamedTypeSymbol namedTypeSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      if (options.TryGetCached(namedTypeSymbol, out NamedTypeSymbolArchetype cached))
      {
         return cached;
      }
      
      var symbolSpec = SymbolSpecTransformer.Transform(namedTypeSymbol, depth, options);
      var typeSpec = TypeSymbolSpecTransformer.Transform(namedTypeSymbol, depth, options);
      var namedSpec = NamedTypeSymbolSpecTransformer.Transform(namedTypeSymbol, depth, options);
      
      var archetype = new NamedTypeSymbolArchetype(symbolSpec, typeSpec, namedSpec);
      options.AddToCache(namedTypeSymbol, archetype);
      
      return archetype;
   }
}
