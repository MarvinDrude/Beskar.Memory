using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for base symbol specs, defining whether attributes or other details should be loaded.
/// </summary>
public sealed class SymbolTransformOptions 
   : SymbolBaseTransformOptions<SymbolLoadFlags>
{
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="SymbolTransformOptions"/> instance.</returns>
   public SymbolTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal base symbol information (attributes are not loaded).
   /// </summary>
   public static SymbolTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new SymbolLoadFlags()
      {
         Attributes = false,
      }
   };
   
   /// <summary>
   /// Gets a set of options that fully loads base symbol information with infinite recursive depth (attributes are not loaded to avoid duplicate effort).
   /// </summary>
   public static SymbolTransformOptions Full => new()
   {
      Depth = int.MaxValue,
      Load = new SymbolLoadFlags()
      {
         Attributes = false, // it's a duplication often
      }
   };
}
