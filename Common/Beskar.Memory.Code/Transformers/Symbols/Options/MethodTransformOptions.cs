using Beskar.Memory.Code.Models.Symbols;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public sealed class MethodTransformOptions
   : SymbolBaseTransformOptions<MethodSymbolLoadFlags>
{
   public Func<IParameterSymbol, bool>? ParameterFilter { get; set; }
 
   public MethodTransformOptions WithDepth(int depth)
   {
      Depth = depth;
      return this;
   }
   
   public static MethodTransformOptions Minimal => new()
   {
      Depth = 1,
      Load = new MethodSymbolLoadFlags()
      {
         Parameters = false,
         ReturnType = false,
         Attributes = false,
      }
   };
   
   public static MethodTransformOptions Full => new()
   {
      Depth = 1,
      Load = new MethodSymbolLoadFlags()
      {
         Parameters = true,
         ReturnType = true,
         Attributes = true,
      }
   };
}

