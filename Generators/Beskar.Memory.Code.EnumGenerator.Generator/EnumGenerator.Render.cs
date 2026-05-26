using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.EnumGenerator.Generator.Models;
using Beskar.Memory.Code.EnumGenerator.Generator.Rendering;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.EnumGenerator.Generator;

public sealed partial class EnumGenerator
{
   private static void Render(
      SourceProductionContext context,
      string assemblyName,
      MaybeSpec<FastEnumSpec> maybeSpec)
   {
      context.DispatchDiagnostics(Diagnostics, maybeSpec);
      if (!maybeSpec.HasValue)
      {
         return;
      }

      var ct = context.CancellationToken;
      ct.ThrowIfCancellationRequested();

      var renderer = new EnumExtensionRenderer(context)
      {
         Spec = maybeSpec.Value
      };
      
      var nameSpace = maybeSpec.Value.NamedType.Symbol.NameSpace
         is { Length: > 0 } ns ? ns : "Global.";
      
      renderer.Render($"{nameSpace}.{maybeSpec.Value.NamedType.Symbol.Name}Extensions.g.cs");
   }
}
