extern alias Generator;

using Beskar.Memory.Writers;
using Beskar.Memory.Code.PacketGenerator.Attributes;
using Beskar.Memory.Code.PacketGenerator.Tests.Constants;
using Beskar.Memory.Tests.Utils;
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

      Assert.Equal(2, result.Diagnostics.Length);
      Assert.Empty(result.GeneratedDiagnostics);
      Assert.Equal(4, result.GeneratedSyntaxTrees.Length);
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
