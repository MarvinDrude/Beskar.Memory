using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class FieldSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IFieldSymbol
   {
      public FieldSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return FieldSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}

