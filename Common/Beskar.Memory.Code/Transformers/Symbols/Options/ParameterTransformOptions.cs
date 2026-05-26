using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for parameter symbols, defining what components to load.
/// </summary>
public sealed class ParameterTransformOptions 
   : SymbolBaseTransformOptions<ParameterSymbolLoadFlags>
{
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="ParameterTransformOptions"/> instance.</returns>
   public ParameterTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal parameter information (no type or attributes loaded).
   /// </summary>
   public static ParameterTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new ParameterSymbolLoadFlags()
      {
         Type = false,
         Attributes = false,
      }
   };
   
   /// <summary>
   /// Gets a set of options that fully loads all parameter information (type and attributes loaded).
   /// </summary>
   public static ParameterTransformOptions Full => new()
   {
      Depth = 1,
      Load = new ParameterSymbolLoadFlags()
      {
         Type = true,
         Attributes = true,
      }
   };
}
