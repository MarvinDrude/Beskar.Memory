using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="IFieldSymbol"/> to construct archetype models.
/// </summary>
public static class FieldSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IFieldSymbol
   {
      /// <summary>
      /// Creates a <see cref="FieldSymbolArchetype"/> from the field symbol using the specified options.
      /// </summary>
      /// <param name="options">The options to configure archetype transformation.</param>
      /// <returns>A constructed <see cref="FieldSymbolArchetype"/> representing the field symbol.</returns>
      public FieldSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return FieldSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}
