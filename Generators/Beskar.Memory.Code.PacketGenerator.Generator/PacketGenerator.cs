using Beskar.Memory.Code.PacketGenerator.Generator.Models;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.PacketGenerator.Generator;

[Generator]
public sealed partial class PacketGenerator : IIncrementalGenerator
{
   public const string GeneratorName = "PacketGenerator";
   public const string GeneratorVersion = "1.5.2";

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var assemblyNameProvider = context.CompilationProvider
         .Select(static (c, _) => c.AssemblyName?
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty)
            .Replace("-", string.Empty)
            .Trim() ?? "UnknownAssembly");

      var maybePacketSpecProvider = context.SyntaxProvider
         .ForAttributeWithMetadataName(
            PacketAttributeFullName,
            predicate: static (_, _) => true,
            transform: Transform);

      var maybePacketRegistrySpecProvider = context.SyntaxProvider
         .ForAttributeWithMetadataName(
            PacketRegistryAttributeFullName,
            predicate: static (_, _) => true,
            transform: TransformRegistry);
      var maybePacketRegistryGenericSpecProvider = context.SyntaxProvider
         .ForAttributeWithMetadataName(
            PacketRegistryGenericAttributeFullName,
            predicate: static (_, _) => true,
            transform: TransformRegistry);

      var combinedProvider = maybePacketRegistrySpecProvider
         .Collect()
         .Combine(maybePacketRegistryGenericSpecProvider.Collect())
         .SelectMany(static (pair, _) => {
            var (nonGeneric, generic) = pair;
            return nonGeneric.Concat(generic);
         });

      var packetCombined = maybePacketSpecProvider
         .Collect().Combine(assemblyNameProvider);

      var registryProvider = combinedProvider
         .Combine(packetCombined)
         .Select(static (combined, _) =>
         {
            var registry = combined.Left;
            var packets = combined.Right.Left
               .Where(x => x.HasValue)
               .Select(x => x.Value)
               .ToArray();

            var registryValue = registry.Value;
            var registryFullName = registryValue.NamedTypeArchetype.Symbol.FullName;

            var relevantIndices = packets
               .Select((packet, index) => new { packet, index })
               .Where(x => x.packet.RegistryFullTypeNames.Array.Contains(registryFullName))
               .Select(x => x.index)
               .ToArray();

            var specs = new SequenceArray<int>(relevantIndices);
            var packetSpecs = new SequenceArray<PacketSpec>(packets);

            return (registry, packetSpecs, specs);
         });
      var registryCombined = registryProvider.Combine(assemblyNameProvider);

      context.RegisterSourceOutput(packetCombined, static (ctx, source)
         => RenderPackets(ctx, source.Right, source.Left));

      context.RegisterSourceOutput(registryCombined, static (ctx, source)
         => RenderRegistry(ctx, source.Right, source.Left.registry, source.Left.packetSpecs, source.Left.specs));

      context.RegisterPostInitializationOutput(static ctx =>
      {
         ctx.AddSource($"{GeneratorName}.g.cs", $"// Version {GeneratorVersion}");
      });
   }
}
