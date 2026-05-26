using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public sealed class TypeTransformOptions 
   : SymbolBaseTransformOptions<TypeSymbolLoadFlags>
{
   public TypeTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
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

