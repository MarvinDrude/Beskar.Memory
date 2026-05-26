using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

/// <summary>
/// Provides transformation methods to convert an <see cref="ITypeSymbol"/> into a <see cref="TypeSymbolArchetype"/>.
/// </summary>
public static class TypeSymbolArchetypeTransformer
{
   /// <summary>
   /// Transforms the specified compiler type symbol into a type symbol archetype representation.
   /// </summary>
   /// <param name="typeSymbol">The compiler type symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="TypeSymbolArchetype"/>.</returns>
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
