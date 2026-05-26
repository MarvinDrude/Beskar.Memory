using Beskar.Memory.Code.Models.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for named type symbols, defining what members to load and filtering them.
/// </summary>
public sealed class NamedTypeTransformOptions 
   : SymbolBaseTransformOptions<NamedTypeSymbolLoadFlags>
{
   /// <summary>
   /// Gets or sets an optional filter function to determine which methods should be included.
   /// </summary>
   public Func<IMethodSymbol, bool>? MethodFilter { get; set; }
   
   /// <summary>
   /// Gets or sets an optional filter function to determine which properties should be included.
   /// </summary>
   public Func<IPropertySymbol, bool>? PropertyFilter { get; set; }
   
   /// <summary>
   /// Gets or sets an optional filter function to determine which fields should be included.
   /// </summary>
   public Func<IFieldSymbol, bool>? FieldFilter { get; set; }
   
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="NamedTypeTransformOptions"/> instance.</returns>
   public NamedTypeTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal named type information (no methods, fields, properties, generic arguments or attributes loaded).
   /// </summary>
   public static NamedTypeTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new NamedTypeSymbolLoadFlags()
      {
         Methods = false,
         TypeArgumentNullableAnnotations = false,
         TypeArguments = false,
         TypeParameters = false,
         Attributes = false,
         Properties = false,
         Fields = false,
      }
   };

   /// <summary>
   /// Gets a set of options that fully loads all named type information (methods, fields, properties, generic arguments and attributes loaded).
   /// </summary>
   public static NamedTypeTransformOptions Full => new()
   {
      Depth = 1,
      Load = new NamedTypeSymbolLoadFlags()
      {
         Methods = true,
         TypeArgumentNullableAnnotations = true,
         TypeArguments = true,
         TypeParameters = true,
         Attributes = true,
         Properties = true,
         Fields = true,
      }
   };
}
