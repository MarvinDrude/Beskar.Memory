using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes;

/// <summary>
/// Provides transformation methods to convert an <see cref="IPropertySymbol"/> into a <see cref="PropertySymbolArchetype"/>.
/// </summary>
public static class PropertySymbolArchetypeTransformer
{
   /// <summary>
   /// Transforms the specified compiler property symbol into a property symbol archetype representation.
   /// </summary>
   /// <param name="propertySymbol">The compiler property symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="PropertySymbolArchetype"/>.</returns>
   public static PropertySymbolArchetype Transform(
      IPropertySymbol propertySymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();

      if (options.TryGetCached(propertySymbol, out PropertySymbolArchetype cached))
      {
         var needsType = options.Properties.Load.Type && depth <= options.Properties.Depth;
         var hasType = cached.Property.IsTypeLoaded;

         if (!needsType || hasType)
         {
            return cached;
         }
      }

      var symbolSpec = SymbolSpecTransformer.Transform(propertySymbol, depth, options);
      var propertySpec = PropertySymbolSpecTransformer.Transform(propertySymbol, depth, options);

      var archetype = new PropertySymbolArchetype(symbolSpec, propertySpec);
      options.AddToCache(propertySymbol, archetype);

      return archetype;
   }
}
