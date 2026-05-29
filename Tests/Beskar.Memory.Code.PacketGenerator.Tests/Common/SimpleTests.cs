extern alias Generator;

using System.Linq;
using Beskar.Memory.Writers;
using Beskar.Memory.Code.PacketGenerator.Attributes;
using Beskar.Memory.Code.PacketGenerator.Tests.Constants;
using Beskar.Memory.Tests.Utils;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Beskar.Memory.Code.PacketGenerator.Tests.Common;

public sealed class SimpleTests
{
   [Fact]
   public void RegistryTwoTest()
   {
      var result = Compilations.Create()
         .AddTestScenario("Scenarios/Simple")
         .WithReferenceByType<PacketAttribute>()
         .WithReferenceByType<ArrayBuilder<object>>()
         .AddSourceGenerator(new Generator::Beskar.Memory.Code.PacketGenerator.Generator.PacketGenerator())
         .Create();
      
      var debugReport = result.GetDebugReport();
      var compilation = result.Compilation;

      var exampleRegistryTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("ExampleRegistry.g.cs"));
      Assert.NotNull(exampleRegistryTree);
      var exampleRegistrySource = exampleRegistryTree.ToString();
      Assert.Equal(2, result.Diagnostics.Length);
      Assert.Empty(result.GeneratedDiagnostics);
      Assert.Equal(4, result.GeneratedSyntaxTrees.Length);

      Assert.Contains("new Beskar_Memory_Code_PacketGenerator_Tests_Scenarios_Simple_ClusterPacket_global__Beskar_Memory_Code_PacketGenerator_Tests_Scenarios_Simple_WrappedPingPayload_HandlerCollection(this)", exampleRegistrySource);
      Assert.Contains("file sealed class Beskar_Memory_Code_PacketGenerator_Tests_Scenarios_Simple_ClusterPacket_global__Beskar_Memory_Code_PacketGenerator_Tests_Scenarios_Simple_WrappedPingPayload_HandlerCollection", exampleRegistrySource);
      Assert.Contains("BasePacketHandlerCollection<object, global::Beskar.Memory.Code.PacketGenerator.Tests.Scenarios.Simple.ClusterPacket<global::Beskar.Memory.Code.PacketGenerator.Tests.Scenarios.Simple.WrappedPingPayload>>", exampleRegistrySource);

      var metadataInitializerTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("TestAssembly.PacketMetadata.g.cs"));
      Assert.NotNull(metadataInitializerTree);
      var metadataInitializerSource = metadataInitializerTree.ToString();
      Assert.Contains("PacketMetadata<global::Beskar.Memory.Code.PacketGenerator.Tests.Scenarios.Simple.ClusterPacket<global::Beskar.Memory.Code.PacketGenerator.Tests.Scenarios.Simple.WrappedPingPayload>>.Identifier =", metadataInitializerSource);
   }

   [Fact]
   public void StatefulRegistryTest()
   {
      var result = Compilations.Create()
         .AddTestScenario("Scenarios/Stateful")
         .WithReferenceByType<PacketAttribute>()
         .WithReferenceByType<ArrayBuilder<object>>()
         .AddSourceGenerator(new Generator::Beskar.Memory.Code.PacketGenerator.Generator.PacketGenerator())
         .Create();
      
      var debugReport = result.GetDebugReport();
      var compilation = result.Compilation;

      Assert.Single(result.Diagnostics);
      Assert.Empty(result.GeneratedDiagnostics);
      Assert.Equal(3, result.GeneratedSyntaxTrees.Length);
   }
}
