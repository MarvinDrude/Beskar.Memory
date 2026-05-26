using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

public static class NamedTypeSymbolArchetypeTransformer
{
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

