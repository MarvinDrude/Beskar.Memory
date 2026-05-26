using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Flags;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public sealed class FieldTransformOptions 
   : SymbolBaseTransformOptions<FieldSymbolLoadFlags>
{
   public FieldTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   public static FieldTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new FieldSymbolLoadFlags()
      {
         Type = false,
         Attributes = false,
      }
   };

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

