using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="IParameterSymbol"/> to construct parameter archetype models.
/// </summary>
public static class ParameterSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IParameterSymbol
   {
      /// <summary>
      /// Creates a <see cref="ParameterSymbolArchetype"/> from the parameter symbol using the specified options.
      /// </summary>
      /// <param name="options">The options to configure archetype transformation.</param>
      /// <returns>A constructed <see cref="ParameterSymbolArchetype"/> representing the parameter symbol.</returns>
      public ParameterSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return ParameterSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}
