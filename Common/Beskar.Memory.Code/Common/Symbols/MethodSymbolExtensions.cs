using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class MethodSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : IMethodSymbol
   {
      public MethodSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return MethodSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}

