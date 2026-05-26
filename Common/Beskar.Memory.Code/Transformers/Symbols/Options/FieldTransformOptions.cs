using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Flags;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for field symbols, defining what data components should be loaded.
/// </summary>
public sealed class FieldTransformOptions 
   : SymbolBaseTransformOptions<FieldSymbolLoadFlags>
{
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="FieldTransformOptions"/> instance.</returns>
   public FieldTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal field information (no types or attributes loaded).
   /// </summary>
   public static FieldTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new FieldSymbolLoadFlags()
      {
         Type = false,
         Attributes = false,
      }
   };

   /// <summary>
   /// Gets a set of options that fully loads all field information (types and attributes loaded).
   /// </summary>
   public static FieldTransformOptions Full => new()
   {
      Depth = 1,
      Load = new FieldSymbolLoadFlags()
      {
         Type = true,
         Attributes = true,
      }
   };
}
