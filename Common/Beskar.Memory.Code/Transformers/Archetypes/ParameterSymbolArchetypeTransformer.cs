using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

public static class ParameterSymbolArchetypeTransformer
{
   public static ParameterSymbolArchetype Transform(
      IParameterSymbol parameterSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      if (options.TryGetCached(parameterSymbol, out ParameterSymbolArchetype cached))
      {
         return cached;
      }
      
      var symbolSpec = SymbolSpecTransformer.Transform(parameterSymbol, depth, options);
      var parameterSpec = ParameterSymbolSpecTransformer.Transform(parameterSymbol, depth, options);
      
      var archetype = new ParameterSymbolArchetype(symbolSpec, parameterSpec);
      options.AddToCache(parameterSymbol, archetype);
      
      return archetype;
   }
}

