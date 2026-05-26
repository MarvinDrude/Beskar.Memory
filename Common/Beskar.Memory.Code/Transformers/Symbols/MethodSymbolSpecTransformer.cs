using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols;

/// <summary>
/// Provides transformation methods to convert an <see cref="IMethodSymbol"/> into a <see cref="MethodSymbolSpec"/>.
/// </summary>
public static class MethodSymbolSpecTransformer
{
   /// <summary>
   /// Transforms the specified compiler method symbol into a method symbol specification.
   /// </summary>
   /// <param name="methodSymbol">The compiler method symbol to transform.</param>
   /// <param name="depth">The current depth of recursive transformation.</param>
   /// <param name="options">The transformation options and cache context, or <c>null</c> to use defaults.</param>
   /// <returns>A transformed <see cref="MethodSymbolSpec"/>.</returns>
   public static MethodSymbolSpec Transform(
      IMethodSymbol methodSymbol,
      int depth = 1,
      ArchetypeTransformOptions? options = null)
   {
      options ??= new ArchetypeTransformOptions();
      
      var spec = new MethodSymbolSpec()
      {
         MethodKind = methodSymbol.MethodKind,
         
         HasVoidReturn = methodSymbol.ReturnsVoid,
         IsAsync = methodSymbol.IsAsync,
         IsIterator = methodSymbol.IsIterator,
         IsReadOnly = methodSymbol.IsReadOnly,
         ReturnsByRef = methodSymbol.ReturnsByRef,
         ReturnsByRefReadonly = methodSymbol.ReturnsByRefReadonly,
      };

      if (options.Methods.Load.Attributes)
      {
         spec.Attributes = options.GetAttributes(methodSymbol, methodSymbol.GetAttributes());
      }

      if (depth > options.Methods.Depth)
      {
         return spec;
      }

      if (options.Methods.Load.ReturnType)
      {
         spec.ReturnType = TypeSymbolArchetypeTransformer.Transform(methodSymbol.ReturnType, depth + 1, options);
      }

      if (options.Methods.Load.Parameters)
      {
         spec.Parameters =
            [.. methodSymbol.Parameters
               .Where(x => options.Methods.ParameterFilter is null || options.Methods.ParameterFilter(x))
               .Select(x => ParameterSymbolArchetypeTransformer.Transform(x, depth + 1, options))];
      }
      
      return spec;
   }
}
