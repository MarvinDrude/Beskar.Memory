using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

/// <summary>
/// Provides transformation methods to convert an <see cref="ITypeParameterSymbol"/> into a <see cref="TypeParameterSymbolSpec"/>.
/// </summary>
public static class TypeParameterSymbolSpecTransformer
{
   /// <summary>
   /// Transforms the specified compiler type parameter symbol into a type parameter symbol specification.
   /// </summary>
   /// <param name="typeParameterSymbol">The compiler type parameter symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="TypeParameterSymbolSpec"/>.</returns>
   public static TypeParameterSymbolSpec Transform(
      ITypeParameterSymbol typeParameterSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      var spec = new TypeParameterSymbolSpec()
      {
         Ordinal = typeParameterSymbol.Ordinal,
         
         AllowsRefLikeType = typeParameterSymbol.AllowsRefLikeType,
         HasConstructorConstraint = typeParameterSymbol.HasConstructorConstraint,
         HasReferenceTypeConstraint = typeParameterSymbol.HasReferenceTypeConstraint,
         HasValueTypeConstraint = typeParameterSymbol.HasValueTypeConstraint,
         HasUnmanagedTypeConstraint = typeParameterSymbol.HasUnmanagedTypeConstraint,
         HasNotNullConstraint = typeParameterSymbol.HasNotNullConstraint,
      };

      if (options.TypeParameters.Depth > depth)
      {
         return spec;
      }

      if (options.TypeParameters.Load.ConstraintTypes)
      {
         spec.ConstraintTypes = [..
            typeParameterSymbol.ConstraintTypes
               .Select(constraintType => TypeSymbolArchetypeTransformer.Transform(constraintType, depth + 1, options))
         ];
      }
      
      return spec;
   }
}
