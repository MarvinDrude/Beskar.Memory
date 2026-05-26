using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public sealed class TypeParameterTransformOptions 
   : SymbolBaseTransformOptions<TypeParameterSymbolLoadFlags>
{
   public TypeParameterTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   public static TypeParameterTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new TypeParameterSymbolLoadFlags()
      {
         ConstraintTypes = false,
      }
   };

   public static TypeParameterTransformOptions Full => new()
   {
      Depth = 1,
      Load = new TypeParameterSymbolLoadFlags()
      {
         ConstraintTypes = true
      }
   };
}

