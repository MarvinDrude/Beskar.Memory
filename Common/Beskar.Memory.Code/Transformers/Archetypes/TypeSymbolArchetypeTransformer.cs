using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

public static class TypeSymbolArchetypeTransformer
{
   public static TypeSymbolArchetype Transform(
      ITypeSymbol typeSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      if (options.TryGetCached(typeSymbol, out TypeSymbolArchetype cached))
      {
         return cached;
      }
      
      var symbolSpec = SymbolSpecTransformer.Transform(typeSymbol, depth, options);
      var typeSpec = TypeSymbolSpecTransformer.Transform(typeSymbol, depth, options);

      TypeSymbolArchetype archetype;
      
      if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
      {
         var namedSpec = NamedTypeSymbolSpecTransformer.Transform(namedTypeSymbol, depth, options);
         archetype = new TypeSymbolArchetype(symbolSpec, typeSpec, namedSpec);
      }
      else
      {
         archetype = new TypeSymbolArchetype(symbolSpec, typeSpec, null);
      }
      
      options.AddToCache(typeSymbol, archetype);
      return archetype;
   }
}

