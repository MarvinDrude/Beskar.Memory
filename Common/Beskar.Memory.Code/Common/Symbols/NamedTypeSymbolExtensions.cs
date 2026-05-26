using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class NamedTypeSymbolExtensions
{
   extension<TSymbol>(TSymbol named)
      where TSymbol : INamedTypeSymbol
   {
      public NamedTypeSymbolArchetype CreateNamedArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return NamedTypeSymbolArchetypeTransformer.Transform(named, options: options);
      }
   }
}

