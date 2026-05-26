using Basic.Reference.Assemblies;
using Beskar.Memory.Code.TypeIdGenerator.Tests.Common;

namespace Beskar.Memory.Code.TypeIdGenerator.Tests.Constants;

public static class Compilations
{
   public static TestCompilationCreator Create() =>
      new TestCompilationCreator()
         .WithAssemblyName("Test-Assembly")
         .WithReferences(Net100.References.All)
         .SuppressDiagnostics("CS1591", "CS9113");
}