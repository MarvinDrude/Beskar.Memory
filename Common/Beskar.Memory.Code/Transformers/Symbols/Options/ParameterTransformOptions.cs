using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public sealed class ParameterTransformOptions 
   : SymbolBaseTransformOptions<ParameterSymbolLoadFlags>
{
   public ParameterTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   public static ParameterTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new ParameterSymbolLoadFlags()
      {
         Type = false,
         Attributes = false,
      }
   };
   
   public static ParameterTransformOptions Full => new()
   {
      Depth = 1,
      Load = new ParameterSymbolLoadFlags()
      {
         Type = true,
         Attributes = true,
      }
   };
}

