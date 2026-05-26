using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Specifies transformation options for type parameter symbols, defining whether constraints or other components should be loaded.
/// </summary>
public sealed class TypeParameterTransformOptions 
   : SymbolBaseTransformOptions<TypeParameterSymbolLoadFlags>
{
   /// <summary>
   /// Sets the recursive depth for the transformation options and returns the current instance.
   /// </summary>
   /// <param name="depth">The recursive depth to set.</param>
   /// <returns>The current <see cref="TypeParameterTransformOptions"/> instance.</returns>
   public TypeParameterTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   /// <summary>
   /// Gets a set of options that loads minimal type parameter information (constraint types are not loaded).
   /// </summary>
   public static TypeParameterTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new TypeParameterSymbolLoadFlags()
      {
         ConstraintTypes = false,
      }
   };

   /// <summary>
   /// Gets a set of options that fully loads all type parameter information (constraint types are loaded).
   /// </summary>
   public static TypeParameterTransformOptions Full => new()
   {
      Depth = 1,
      Load = new TypeParameterSymbolLoadFlags()
      {
         ConstraintTypes = true
      }
   };
}
