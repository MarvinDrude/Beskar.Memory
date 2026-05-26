using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

public static class MethodSymbolArchetypeTransformer
{
   public static MethodSymbolArchetype Transform(
      IMethodSymbol methodSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      if (options.TryGetCached(methodSymbol, out MethodSymbolArchetype cached))
      {
         return cached;
      }
      
      var symbolSpec = SymbolSpecTransformer.Transform(methodSymbol, depth, options);
      var methodSpec = MethodSymbolSpecTransformer.Transform(methodSymbol, depth, options);
      
      var archetype = new MethodSymbolArchetype(symbolSpec, methodSpec);
      options.AddToCache(methodSymbol, archetype);
      
      return archetype;
   }
}

