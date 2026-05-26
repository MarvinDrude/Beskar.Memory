extern alias Generator;

using Beskar.Memory.Writers;
using Beskar.Memory.Code.EnumGenerator.Attributes;
using Beskar.Memory.Code.EnumGenerator.Tests.Constants;
using Beskar.Memory.Tests.Utils;
using Xunit;

namespace Beskar.Memory.Code.EnumGenerator.Tests.Common;

public sealed class SimpleTests
{
   [Fact]
   public void RegistryTwoTest()
   {
      var result = Compilations.Create()
         .AddTestScenario("Scenarios/Simple")
         .WithReferenceByType<FastEnumAttribute>()
         .WithReferenceByType<ArrayBuilder<object>>()
         .AddSourceGenerator(new Generator::Beskar.Memory.Code.EnumGenerator.Generator.EnumGenerator())
         .Create();
      
      var debugReport = result.GetDebugReport();
      var compilation = result.Compilation;

      Assert.Empty(result.Diagnostics);
      Assert.Empty(result.GeneratedDiagnostics);
   }
}
