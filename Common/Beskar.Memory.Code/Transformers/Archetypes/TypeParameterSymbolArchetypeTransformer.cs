using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

/// <summary>
/// Provides transformation methods to convert an <see cref="ITypeParameterSymbol"/> into a <see cref="TypeParameterArchetype"/>.
/// </summary>
public static class TypeParameterSymbolArchetypeTransformer
{
   /// <summary>
   /// Transforms the specified compiler type parameter symbol into a type parameter archetype representation.
   /// </summary>
   /// <param name="typeParameterSymbol">The compiler type parameter symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="TypeParameterArchetype"/>.</returns>
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
