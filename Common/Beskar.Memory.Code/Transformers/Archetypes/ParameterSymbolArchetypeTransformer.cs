using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

/// <summary>
/// Provides transformation methods to convert an <see cref="IParameterSymbol"/> into a <see cref="ParameterSymbolArchetype"/>.
/// </summary>
public static class ParameterSymbolArchetypeTransformer
{
   /// <summary>
   /// Transforms the specified compiler parameter symbol into a parameter symbol archetype representation.
   /// </summary>
   /// <param name="parameterSymbol">The compiler parameter symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="ParameterSymbolArchetype"/>.</returns>
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
