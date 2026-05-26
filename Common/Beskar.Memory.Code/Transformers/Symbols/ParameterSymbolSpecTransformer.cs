using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

/// <summary>
/// Provides transformation methods to convert an <see cref="IParameterSymbol"/> into a <see cref="ParameterSymbolSpec"/>.
/// </summary>
public static class ParameterSymbolSpecTransformer
{
   /// <summary>
   /// Transforms the specified compiler parameter symbol into a parameter symbol specification.
   /// </summary>
   /// <param name="parameterSymbol">The compiler parameter symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="ParameterSymbolSpec"/>.</returns>
   public static ParameterSymbolSpec Transform(
      IParameterSymbol parameterSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      var spec = new ParameterSymbolSpec()
      {
         Ordinal = parameterSymbol.Ordinal,
         ScopeKind = parameterSymbol.ScopedKind,
         RefKind = parameterSymbol.RefKind,
         
         HasExplicitDefaultValue = parameterSymbol.HasExplicitDefaultValue,
         IsParamsArray = parameterSymbol.IsParamsArray,
         IsParamsCollection = parameterSymbol.IsParamsCollection,
         IsDiscard = parameterSymbol.IsDiscard,
         IsOptional = parameterSymbol.IsOptional
      };

      if (options.Parameters.Load.Attributes)
      {
         spec.Attributes = options.GetAttributes(parameterSymbol, parameterSymbol.GetAttributes());
      }

      if (depth > options.Parameters.Depth)
      {
         return spec;
      }
      
      if (options.Parameters.Load.Type)
      {
         spec.Type = TypeSymbolArchetypeTransformer.Transform(parameterSymbol.Type, depth + 1, options);
      }
      
      return spec;
   }
}
