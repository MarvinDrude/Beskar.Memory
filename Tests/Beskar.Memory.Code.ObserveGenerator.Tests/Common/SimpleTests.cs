extern alias Generator;

using Beskar.Memory.Code.ObserveGenerator.Attributes;
using Beskar.Memory.Code.ObserveGenerator.Tests.Constants;
using Beskar.Memory.Tests.Utils;
using Xunit;

namespace Beskar.Memory.Code.ObserveGenerator.Tests.Common;

public sealed class SimpleTests
{
   [Fact]
   public void ObserveSimple()
   {
      var result = Compilations.Create()
         .AddTestScenario("Scenarios/Simple")
         .WithReferenceByType<ObserveAttribute>()
         .AddSourceGenerator(new Generator::Beskar.Memory.Code.ObserveGenerator.Generator.ObserveGenerator())
         .Create();
      
      var debugReport = result.GetDebugReport();
      var compilation = result.Compilation;

      Assert.Empty(result.Diagnostics);
      Assert.Empty(result.GeneratedDiagnostics);
   }
}
