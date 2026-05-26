using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for type symbols, defining what component relationships (interfaces, bases, attributes) should be loaded.
/// </summary>
public sealed class TypeTransformOptions 
   : SymbolBaseTransformOptions<TypeSymbolLoadFlags>
{
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="TypeTransformOptions"/> instance.</returns>
   public TypeTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal type symbol information (no interfaces, base type or attributes loaded).
   /// </summary>
   public static TypeTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new TypeSymbolLoadFlags()
      {
         AllInterfaces = false,
         BaseType = false,
         Interfaces = false,
         Attributes = false,
      }
   };

   /// <summary>
   /// Gets a set of options that fully loads all type symbol information (interfaces, base type and attributes loaded).
   /// </summary>
   public static TypeTransformOptions Full => new()
   {
      Depth = 1,
      Load = new TypeSymbolLoadFlags()
      {
         AllInterfaces = true,
         BaseType = true,
         Interfaces = true,
         Attributes = true,
      }
   };
}
