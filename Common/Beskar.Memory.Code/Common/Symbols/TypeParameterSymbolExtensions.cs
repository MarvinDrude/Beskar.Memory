using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class TypeParameterSymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : ITypeParameterSymbol
   {
      public TypeParameterArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return TypeParameterSymbolArchetypeTransformer.Transform(symbol, options: options);
      }
   }
}

