using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="IMethodSymbol"/> to construct archetype models.
/// </summary>
public static class MethodSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IMethodSymbol
   {
      /// <summary>
      /// Creates a <see cref="MethodSymbolArchetype"/> from the method symbol using the specified options.
      /// </summary>
      /// <param name="options">The options to configure archetype transformation.</param>
      /// <returns>A constructed <see cref="MethodSymbolArchetype"/> representing the method symbol.</returns>
      public MethodSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return MethodSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}
