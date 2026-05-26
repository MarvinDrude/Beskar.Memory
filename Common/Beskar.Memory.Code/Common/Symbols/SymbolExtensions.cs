using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="ISymbol"/> to construct specification models and analyze interface implementations.
/// </summary>
public static class SymbolExtensions
{
   extension<TSymbol>(TSymbol symbol)
      where TSymbol : ISymbol
   {
      /// <summary>
      /// Creates a <see cref="SymbolSpec"/> representing the specifications of the symbol.
      /// </summary>
      /// <param name="options">The options to configure symbol spec transformation.</param>
      /// <returns>A <see cref="SymbolSpec"/> containing the symbol metadata.</returns>
      public SymbolSpec CreateSpec(ArchetypeTransformOptions? options = null)
      {
         return SymbolSpecTransformer.Transform(symbol, options: options);
      }
      
      /// <summary>
      /// Gets a list of interface members that this symbol explicitly or implicitly implements.
      /// </summary>
      /// <returns>A list of <see cref="ISymbol"/> objects representing the interface members that this symbol implements.</returns>
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
