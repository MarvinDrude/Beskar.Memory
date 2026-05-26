using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public sealed class SymbolTransformOptions 
   : SymbolBaseTransformOptions<SymbolLoadFlags>
{
   public SymbolTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   public static SymbolTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new SymbolLoadFlags()
      {
         Attributes = false,
      }
   };
   
   public static SymbolTransformOptions Full => new()
   {
      Depth = int.MaxValue,
      Load = new SymbolLoadFlags()
      {
         Attributes = false, // it's a duplication often
      }
   };
}

