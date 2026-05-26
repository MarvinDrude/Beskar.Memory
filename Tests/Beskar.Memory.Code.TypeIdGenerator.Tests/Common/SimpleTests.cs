extern alias Generator;

using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Code.TypeIdGenerator.Attributes;
using Beskar.Memory.Code.TypeIdGenerator.Tests.Constants;
using Beskar.Memory.Tests.Utils;

namespace Beskar.Memory.Code.TypeIdGenerator.Tests.Common;

public sealed class SimpleTests
{
   [Fact]
   public void UnderlyingInt()
   {
      var result = Compilations.Create()
         .AddTestScenario("Scenarios/Simple")
         .WithReferenceByType<TypeSafeIdAttribute>()
         .AddSourceGenerator(new Generator::Beskar.Memory.Code.TypeIdGenerator.Generator.TypeIdGenerator())
         .Create();
      
      var compilation = result.Compilation;

      Assert.Empty(result.Diagnostics);
      Assert.Empty(result.GeneratedDiagnostics);
   }
}