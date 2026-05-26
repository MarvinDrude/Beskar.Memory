using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

/// <summary>
/// Provides transformation methods to convert an <see cref="IMethodSymbol"/> into a <see cref="MethodSymbolArchetype"/>.
/// </summary>
public static class MethodSymbolArchetypeTransformer
{
   /// <summary>
   /// Transforms the specified compiler method symbol into a method symbol archetype representation.
   /// </summary>
   /// <param name="methodSymbol">The compiler method symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="MethodSymbolArchetype"/>.</returns>
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
