using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public sealed class PropertyTransformOptions 
   : SymbolBaseTransformOptions<PropertySymbolLoadFlags>
{
   public PropertyTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
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

