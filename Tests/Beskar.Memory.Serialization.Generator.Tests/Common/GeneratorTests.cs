extern alias Generator;

using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Beskar.Memory.Serialization.Attributes;
using Beskar.Memory.Serialization.Generator.Tests.Constants;
using Beskar.Memory.Tests.Utils;

namespace Beskar.Memory.Serialization.Generator.Tests.Common;

public sealed class GeneratorTests
{
   [Fact]
   public void TestSimpleSerializationGenerator()
   {
      var result = Compilations.Create()
         .AddTestScenario("Scenarios/SimpleSerialization")
         .WithReferenceByType<BeskarObjectAttribute>()
         .WithReferenceByType<Beskar.Memory.Writers.ArrayBuilder<object>>()
         .AddSourceGenerator(new Generator::Beskar.Memory.Code.TypeIdGenerator.Generator.SerializationGenerator())
         .Create();

      Assert.DoesNotContain(result.Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
      Assert.Empty(result.GeneratedDiagnostics);
      Assert.NotEmpty(result.GeneratedSyntaxTrees);

      var simpleClassTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("SimpleClass.g.cs"));
      Assert.NotNull(simpleClassTree);
      var simpleClassSource = simpleClassTree.ToString();
      Assert.Contains("public sealed class SimpleClassSerializer : ISerializer<global::TestNamespace.SimpleClass>", simpleClassSource);
      Assert.Contains("SerializerRegistry<int>.GetWrite()(ref writer, value.Id);", simpleClassSource);
      Assert.Contains("SerializerRegistry<string>.GetWrite()(ref writer, value.Name);", simpleClassSource);
      Assert.Contains("SerializerRegistry<global::TestNamespace.SimpleClass>.Register<SimpleClassSerializer>();", simpleClassSource);

      var simpleStructTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("SimpleStruct.g.cs"));
      Assert.NotNull(simpleStructTree);
      var simpleStructSource = simpleStructTree.ToString();
      Assert.Contains("public sealed class SimpleStructSerializer : ISerializer<global::TestNamespace.SimpleStruct>", simpleStructSource);
      Assert.DoesNotContain("if (value is null)", simpleStructSource);
      Assert.Contains("SerializerRegistry<double>.GetWrite()(ref writer, value.Value);", simpleStructSource);

      var ignoredClassTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("IgnoredMemberClass.g.cs"));
      Assert.NotNull(ignoredClassTree);
      var ignoredClassSource = ignoredClassTree.ToString();
      Assert.Contains("SerializerRegistry<int>.GetWrite()(ref writer, value.Kept);", ignoredClassSource);
      Assert.DoesNotContain("value.Ignored", ignoredClassSource);
      Assert.DoesNotContain("value.IgnoredWithOrder", ignoredClassSource);

      var recordTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("ImmutableRecord.g.cs"));
      Assert.NotNull(recordTree);
      var recordSource = recordTree.ToString();
      Assert.Contains("value = new global::TestNamespace.ImmutableRecord(member_Id, member_Data);", recordSource);

      var unionTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("UnionBase.g.cs"));
      Assert.NotNull(unionTree);
      var unionSource = unionTree.ToString();
      Assert.Contains("if (value is global::TestNamespace.UnionChild child1)", unionSource);
      Assert.Contains("(1).WriteLittleEndian(ref writer);", unionSource);
      Assert.Contains("if (tag == 1)", unionSource);

      var nonPolyChildTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("NonPolyChild.g.cs"));
      Assert.NotNull(nonPolyChildTree);
      var nonPolyChildSource = nonPolyChildTree.ToString();
      Assert.Contains("public sealed class NonPolyChildSerializer : ISerializer<global::TestNamespace.NonPolyChild>", nonPolyChildSource);
      Assert.Contains("SerializerRegistry<int>.GetWrite()(ref writer, value.ChildValue);", nonPolyChildSource);
   }
}
