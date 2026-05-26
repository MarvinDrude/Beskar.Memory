using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

/// <summary>
/// Provides transformation methods to convert an <see cref="IFieldSymbol"/> into a <see cref="FieldSymbolArchetype"/>.
/// </summary>
public static class FieldSymbolArchetypeTransformer
{
   /// <summary>
   /// Transforms the specified compiler field symbol into a field symbol archetype representation.
   /// </summary>
   /// <param name="fieldSymbol">The compiler field symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="FieldSymbolArchetype"/>.</returns>
   public static FieldSymbolArchetype Transform(
      IFieldSymbol fieldSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();

      if (options.TryGetCached(fieldSymbol, out FieldSymbolArchetype cached))
      {
         return cached;
      }
      
      var symbolSpec = SymbolSpecTransformer.Transform(fieldSymbol, depth, options);
      var fieldSpec = FieldSymbolSpecTransformer.Transform(fieldSymbol, depth, options);
      
      var archetype = new FieldSymbolArchetype(symbolSpec, fieldSpec);
      options.AddToCache(fieldSymbol, archetype);
      
      return archetype;
   }
}
