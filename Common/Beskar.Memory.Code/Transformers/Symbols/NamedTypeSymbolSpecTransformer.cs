using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

/// <summary>
/// Provides transformation methods to convert an <see cref="INamedTypeSymbol"/> into a <see cref="NamedTypeSymbolSpec"/>.
/// </summary>
public static class NamedTypeSymbolSpecTransformer
{
   /// <summary>
   /// Transforms the specified compiler named type symbol into a named type symbol specification.
   /// </summary>
   /// <param name="namedTypeSymbol">The compiler named type symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="NamedTypeSymbolSpec"/>.</returns>
   public static NamedTypeSymbolSpec Transform(
      INamedTypeSymbol namedTypeSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      var spec = new NamedTypeSymbolSpec()
      {
         IsEnum = namedTypeSymbol.TypeKind == TypeKind.Enum,
         IsFileLocal = namedTypeSymbol.IsFileLocal,
         Arity = namedTypeSymbol.Arity,
      };
      
      if (options.NamedTypes.Load.Attributes)
      {
         spec.Attributes = options.GetAttributes(namedTypeSymbol, namedTypeSymbol.GetAttributes());
      }

      if (depth > options.NamedTypes.Depth)
      {
         return spec;
      }

      if (options.NamedTypes.Load.Methods)
      {
         spec.Methods = [.. 
            namedTypeSymbol
               .GetMembers()
               .OfType<IMethodSymbol>()
               .Where(m => options.NamedTypes.MethodFilter is null || options.NamedTypes.MethodFilter(m))
               .Select(m => MethodSymbolArchetypeTransformer.Transform(m, depth + 1, options))
         ];
      }
      
      if (options.NamedTypes.Load.Fields)
      {
         spec.Fields = [.. 
            namedTypeSymbol
               .GetMembers()
               .OfType<IFieldSymbol>()
               .Where(m => options.NamedTypes.FieldFilter is null || options.NamedTypes.FieldFilter(m))
               .Select(m => FieldSymbolArchetypeTransformer.Transform(m, depth + 1, options))
         ];
      }

      if (options.NamedTypes.Load.Properties)
      {
         spec.Properties = [.. 
            namedTypeSymbol
               .GetMembers()
               .OfType<IPropertySymbol>()
               .Where(m => options.NamedTypes.PropertyFilter is null || options.NamedTypes.PropertyFilter(m))
               .Select(m => PropertySymbolArchetypeTransformer.Transform(m, depth + 1, options))
         ];
      }
      
      if (options.NamedTypes.Load.TypeParameters)
      {
         spec.TypeParameters = [.. 
            namedTypeSymbol
               .TypeParameters
               .Select(tp => TypeParameterSymbolArchetypeTransformer.Transform(tp, depth + 1, options))
         ];
      }

      if (options.NamedTypes.Load.TypeArguments)
      {
         spec.TypeArguments = [.. 
            namedTypeSymbol
               .TypeArguments
               .Select(ta => TypeSymbolArchetypeTransformer.Transform(ta, depth + 1, options))
         ];
      }
      
      if (options.NamedTypes.Load.TypeArgumentNullableAnnotations)
      {
         spec.TypeArgumentNullableAnnotations = [.. 
            namedTypeSymbol
               .TypeArguments
               .Select(ta => ta.NullableAnnotation)
         ];
      }

      return spec;
   }
}
