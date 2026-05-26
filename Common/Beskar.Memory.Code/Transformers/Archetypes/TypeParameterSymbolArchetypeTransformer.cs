using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

public static class TypeParameterSymbolArchetypeTransformer
{
   public static TypeParameterArchetype Transform(
      ITypeParameterSymbol typeParameterSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      if (options.TryGetCached(typeParameterSymbol, out TypeParameterArchetype cached))
      {
         return cached;
      }
      
      var symbolSpec = SymbolSpecTransformer.Transform(typeParameterSymbol, depth, options);
      var typeParameterSpec = TypeParameterSymbolSpecTransformer.Transform(typeParameterSymbol, depth, options);
      
      var archetype = new TypeParameterArchetype(symbolSpec, typeParameterSpec);
      options.AddToCache(typeParameterSymbol, archetype);
      
      return archetype;
   }
}

