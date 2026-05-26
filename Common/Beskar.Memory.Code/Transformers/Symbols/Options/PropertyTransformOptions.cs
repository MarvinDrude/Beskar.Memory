using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for property symbols, defining what accessors and components to load.
/// </summary>
public sealed class PropertyTransformOptions 
   : SymbolBaseTransformOptions<PropertySymbolLoadFlags>
{
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="PropertyTransformOptions"/> instance.</returns>
   public PropertyTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal property information (no type, accessors, or attributes loaded).
   /// </summary>
   public static PropertyTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new PropertySymbolLoadFlags()
      {
         Type = false,
         Getter = false,
         Setter = false,
         Attributes = false,
      }
   };
   
   /// <summary>
   /// Gets a set of options that fully loads all property information (type, accessors, and attributes loaded).
   /// </summary>
   public static PropertyTransformOptions Full => new()
   {
      Depth = 1,
      Load = new PropertySymbolLoadFlags()
      {
         Type = true,
         Getter = true,
         Setter = true,
         Attributes = true,
      }
   };
}
