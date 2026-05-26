using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class SymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : ISymbol
   {
      public SymbolSpec CreateSpec(ArchetypeTransformOptions? options = null)
      {
         return SymbolSpecTransformer.Transform(symbol, options: options);
      }
      
      public List<ISymbol> ExplicitOrImplicitInterfaceImplementations()
      {
         if (symbol.Kind != SymbolKind.Method &&
             symbol.Kind != SymbolKind.Property &&
             symbol.Kind != SymbolKind.Event)
         {
            return [];
         }
      
         var containingType = symbol.ContainingType;

         return containingType
            .AllInterfaces
            .SelectMany(iface => iface.GetMembers())
            .Where(interfaceMember =>
               SymbolEqualityComparer.Default.Equals(
                  symbol, 
                  containingType.FindImplementationForInterfaceMember(interfaceMember)))
            .ToList();
      }
   }
}

