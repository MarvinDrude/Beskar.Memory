using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

public static class SymbolSpecTransformer
{
   public static SymbolSpec Transform<TSymbol>(
      TSymbol symbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
      where TSymbol : ISymbol
   {
      options ??= new ArchetypeTransformOptions();
      
      var nameSpace = symbol.ContainingNamespace?.ToDisplayString();
      if (nameSpace == "<global namespace>")
      {
         nameSpace = null;
      }
      
      var spec = new SymbolSpec()
      {
         Accessibility = symbol.DeclaredAccessibility,
         Kind = symbol.Kind,
         
         Name = symbol.Name,
         MetadataName = symbol.MetadataName,
         FullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
         NameSpace = nameSpace,
         
         IsStatic = symbol.IsStatic,
         IsAbstract = symbol.IsAbstract,
         IsSealed = symbol.IsSealed,
         IsVirtual = symbol.IsVirtual,
         IsExtern = symbol.IsExtern,
         IsOverride = symbol.IsOverride,
         IsImplicitlyDeclared = symbol.IsImplicitlyDeclared,
      };

      if (options.Symbols.Load.Attributes)
      {
         spec.Attributes = options.GetAttributes(symbol, symbol.GetAttributes());
      }
      
      return spec;
   }
}

