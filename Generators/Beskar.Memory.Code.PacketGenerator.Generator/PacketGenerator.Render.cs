using System.Collections.Immutable;
using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Specs;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.PacketGenerator.Generator.Models;
using Beskar.Memory.Code.PacketGenerator.Generator.Rendering;
using Beskar.Memory.Writers;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.PacketGenerator.Generator;

public sealed partial class PacketGenerator
{
   private static void RenderRegistry(
      SourceProductionContext context,
      string assemblyName,
      MaybeSpec<PacketRegistrySpec> registrySpec,
      SequenceArray<PacketSpec> packetSpecs,
      SequenceArray<int> packetIndices)
   {
      context.DispatchDiagnostics(Diagnostics, registrySpec);
      if (!registrySpec.HasValue)
      {
         return;
      }
      
      var ct = context.CancellationToken;
      ct.ThrowIfCancellationRequested();

      var renderer = new PacketRegistryRenderer(context)
      {
         PacketSpecs = packetSpecs,
         PacketIndices = packetIndices,
         RegistrySpec = registrySpec.Value,
      };

      renderer.Render(registrySpec.Value.NamedTypeArchetype.Symbol.GeneratedFilePath);
   }

   private static void RenderPackets(
      SourceProductionContext context,
      string assemblyName,
      ImmutableArray<MaybeSpec<PacketSpec>> packetSpecs)
   {
      using var builder = new ArrayBuilder<PacketSpec>(packetSpecs.Length);
      
      foreach (var packetSpec in packetSpecs)
      {
         context.DispatchDiagnostics(Diagnostics, packetSpec);
         if (!packetSpec.HasValue)
         {
            continue;
         }
         
         var ct = context.CancellationToken;
         ct.ThrowIfCancellationRequested();
         
         builder.Add(packetSpec.Value);
      }
      
      var packetRenderer = new PacketRenderer(context)
      {
         PacketSpecs = [..builder.WrittenSpan],
      };
         
      packetRenderer.Render($"{assemblyName}.PacketMetadata.g.cs");
   }
}
