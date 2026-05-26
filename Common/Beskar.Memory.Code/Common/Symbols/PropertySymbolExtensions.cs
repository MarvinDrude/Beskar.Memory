using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class PropertySymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IPropertySymbol
   {
      public PropertySymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return PropertySymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}

