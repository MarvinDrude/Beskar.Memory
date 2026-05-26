using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Specs;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.TypeIdGenerator.Generator.Models;
using Beskar.Memory.Code.TypeIdGenerator.Generator.Rendering;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

public sealed partial class TypeIdGenerator
{
   private static void Render(
      SourceProductionContext context,
      string assemblyName,
      MaybeSpec<TypeSafeIdSpec> maybeSpec)
   {
      context.DispatchDiagnostics(Diagnostics, maybeSpec);
      if (!maybeSpec.HasValue)
      {
         return;
      }
      
      var ct = context.CancellationToken;
      ct.ThrowIfCancellationRequested();

      var renderer = new TypeIdRenderer(context)
      {
         Spec = maybeSpec.Value
      };
      
      renderer.Render(maybeSpec.Value.NamedTargetArchetype.Symbol.GeneratedFilePath);
   }
}