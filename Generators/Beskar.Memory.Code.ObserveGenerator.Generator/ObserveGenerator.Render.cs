using System.Collections.Immutable;
using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Specs;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.ObserveGenerator.Generator.Models;
using Beskar.Memory.Code.ObserveGenerator.Generator.Rendering;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator;

public sealed partial class ObserveGenerator
{
   private static void Render(
      SourceProductionContext context,
      string assemblyName,
      MaybeSpec<ObserveSpec> maybeSpec)
   {
      context.DispatchDiagnostics(Diagnostics, maybeSpec);
      if (!maybeSpec.HasValue)
      {
         return;
      }
      
      var ct = context.CancellationToken;
      ct.ThrowIfCancellationRequested();
      
      var renderer = new ObserveRenderer(context)
      {
         Spec = maybeSpec.Value
      };
      
      renderer.Render(maybeSpec.Value.NamedTypeArchetype.Symbol.GeneratedFilePath);
   }
   
   private static void RenderExtensions(
      SourceProductionContext context,
      string assemblyName,
      ImmutableArray<ObserveSpec> specs)
   {
      var ct = context.CancellationToken;
      ct.ThrowIfCancellationRequested();
      
      var renderer = new ExtensionRenderer(context)
      {
         Specs = specs,
         Namespace = assemblyName
      };
      
      renderer.Render($"{assemblyName}.ObserveExtensions.g.cs");
   }
}
