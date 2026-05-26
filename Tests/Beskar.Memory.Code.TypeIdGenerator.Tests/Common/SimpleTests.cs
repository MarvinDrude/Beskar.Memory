extern alias Generator;

using System.Threading.Tasks;
using Beskar.Memory.Code.TypeIdGenerator.Attributes;
using Beskar.Memory.Code.TypeIdGenerator.Tests.Constants;

namespace Beskar.Memory.Code.TypeIdGenerator.Tests.Common;

public sealed class SimpleTests
{
   [Test]
   public async Task UnderlyingInt()
   {
      var result = Compilations.Create()
         .AddTestScenario("Scenarios/Simple")
         .WithReferenceByType<TypeSafeIdAttribute>()
         .AddSourceGenerator(new Generator::Beskar.Memory.Code.TypeIdGenerator.Generator.TypeIdGenerator())
         .Create();
      
      var debugReport = result.GetDebugReport();
      var compilation = result.Compilation;

      await Assert.That(result.Diagnostics).IsEmpty();
      await Assert.That(result.GeneratedDiagnostics).IsEmpty();
   }
}