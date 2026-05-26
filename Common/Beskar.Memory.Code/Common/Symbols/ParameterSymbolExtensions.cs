using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class ParameterSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IParameterSymbol
   {
      public ParameterSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return ParameterSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}

