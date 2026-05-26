using Beskar.Memory.Code.Models.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for method symbols, defining what components to load and filtering parameters.
/// </summary>
public sealed class MethodTransformOptions
   : SymbolBaseTransformOptions<MethodSymbolLoadFlags>
{
   /// <summary>
   /// Gets or sets an optional filter function to determine which parameters should be included.
   /// </summary>
   public Func<IParameterSymbol, bool>? ParameterFilter { get; set; }
 
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="MethodTransformOptions"/> instance.</returns>
   public MethodTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal method information (no parameters, return types, or attributes loaded).
   /// </summary>
   public static MethodTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new MethodSymbolLoadFlags()
      {
         Parameters = false,
         ReturnType = false,
         Attributes = false,
      }
   };
   
   /// <summary>
   /// Gets a set of options that fully loads all method information (parameters, return types, and attributes loaded).
   /// </summary>
   public static MethodTransformOptions Full => new()
   {
      Depth = 1,
      Load = new MethodSymbolLoadFlags()
      {
         Parameters = true,
         ReturnType = true,
         Attributes = true,
      }
   };
}
