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
      Assert.Contains("public sealed class SimpleClassSerializer : ISerializer<global::TestNamespace.SimpleClass?>", simpleClassSource);
      Assert.Contains("var writeint = SerializerRegistry<int>.GetWrite();", simpleClassSource);
      Assert.Contains("bytesWritten += writeint(ref writer, value.Id);", simpleClassSource);
      Assert.Contains("bytesWritten += writeint(ref writer, value.Id2);", simpleClassSource);
      Assert.Contains("SerializerRegistry<string>.GetWrite()(ref writer, value.Name);", simpleClassSource);
      Assert.Contains("SerializerRegistry<global::TestNamespace.SimpleClass?>.Register<SimpleClassSerializer>();", simpleClassSource);

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
      Assert.Contains("SerializerRegistry<global::TestNamespace.IgnoredMemberClass?>.Register<IgnoredMemberClassSerializer>();", ignoredClassSource);

      var recordTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("ImmutableRecord.g.cs"));
      Assert.NotNull(recordTree);
      var recordSource = recordTree.ToString();
      Assert.Contains("value = new global::TestNamespace.ImmutableRecord(member_Id, member_Data);", recordSource);
      Assert.Contains("SerializerRegistry<global::TestNamespace.ImmutableRecord?>.Register<ImmutableRecordSerializer>();", recordSource);

      var unionTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("UnionBase.g.cs"));
      Assert.NotNull(unionTree);
      var unionSource = unionTree.ToString();
      Assert.Contains("if (value is global::TestNamespace.UnionChild child1)", unionSource);
      Assert.Contains("VarInteger.Write(ref writer, 1);", unionSource);
      Assert.Contains("if (refTag == 1)", unionSource);
      Assert.Contains("SerializerRegistry<global::TestNamespace.UnionBase?>.Register<UnionBaseSerializer>();", unionSource);

      var interfaceUnionTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("IUnionStruct.g.cs"));
      Assert.NotNull(interfaceUnionTree);
      var interfaceUnionSource = interfaceUnionTree.ToString();
      Assert.Contains("if (value is global::TestNamespace.UnionStructA child1)", interfaceUnionSource);
      Assert.Contains("if (value is global::TestNamespace.UnionStructB child2)", interfaceUnionSource);
      Assert.Contains("if (refTag == 1)", interfaceUnionSource);
      Assert.Contains("if (refTag == 2)", interfaceUnionSource);
      Assert.Contains("SerializerRegistry<global::TestNamespace.IUnionStruct?>.Register<IUnionStructSerializer>();", interfaceUnionSource);

      var nonPolyChildTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("NonPolyChild.g.cs"));
      Assert.NotNull(nonPolyChildTree);
      var nonPolyChildSource = nonPolyChildTree.ToString();
      Assert.Contains("public sealed class NonPolyChildSerializer : ISerializer<global::TestNamespace.NonPolyChild?>", nonPolyChildSource);
      Assert.Contains("SerializerRegistry<int>.GetWrite()(ref writer, value.ChildValue);", nonPolyChildSource);
      Assert.Contains("SerializerRegistry<global::TestNamespace.NonPolyChild?>.Register<NonPolyChildSerializer>();", nonPolyChildSource);

      var nonPartialClassTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("NonPartialClass.g.cs"));
      Assert.NotNull(nonPartialClassTree);
      var nonPartialClassSource = nonPartialClassTree.ToString();
      Assert.Contains("public sealed class NonPartialClassSerializer : ISerializer<global::TestNamespace.NonPartialClass?>", nonPartialClassSource);

      var typeWithReadonlyMembersTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("TypeWithReadonlyMembers.g.cs"));
      Assert.NotNull(typeWithReadonlyMembersTree);
      var typeWithReadonlyMembersSource = typeWithReadonlyMembersTree.ToString();
      Assert.DoesNotContain("ReadonlyField", typeWithReadonlyMembersSource);
      Assert.DoesNotContain("ReadonlyProp", typeWithReadonlyMembersSource);
      Assert.Contains("SerializerRegistry<int>.GetWrite()(ref writer, value.Value);", typeWithReadonlyMembersSource);

      var genericPacketTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("GenericPacketWithConstraints.g.cs"));
      Assert.NotNull(genericPacketTree);
      var genericPacketSource = genericPacketTree.ToString();
      Assert.Contains("public sealed class GenericPacketWithConstraintsSerializer<TPacket> : ISerializer<global::TestNamespace.GenericPacketWithConstraints<TPacket>>", genericPacketSource);
      Assert.Contains("where TPacket : struct", genericPacketSource);

      var classWithRequiredMemberTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("ClassWithRequiredMember.g.cs"));
      Assert.NotNull(classWithRequiredMemberTree);
      var classWithRequiredMemberSource = classWithRequiredMemberTree.ToString();
      Assert.Contains("value = new global::TestNamespace.ClassWithRequiredMember", classWithRequiredMemberSource);
      Assert.Contains("RequiredValue = member_RequiredValue", classWithRequiredMemberSource);

      var cyclicNodeTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("CyclicNode.g.cs"));
      Assert.NotNull(cyclicNodeTree);
      var cyclicNodeSource = cyclicNodeTree.ToString();
      Assert.Contains("value = new global::TestNamespace.CyclicNode", cyclicNodeSource);
      Assert.Contains("Name = default!", cyclicNodeSource);
      Assert.Contains("Next = default!", cyclicNodeSource);
      Assert.Contains("context.Register(refTag, value);", cyclicNodeSource);
      Assert.Contains("value.Name = member_Name;", cyclicNodeSource);
      Assert.Contains("value.Next = member_Next;", cyclicNodeSource);
      Assert.Contains("context.IncrementDepth();", cyclicNodeSource);
      Assert.Contains("context.DecrementDepth();", cyclicNodeSource);

      var nullableRefTypesTree = result.GeneratedSyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith("NullableRefTypesClass.g.cs"));
      Assert.NotNull(nullableRefTypesTree);
      var nullableRefTypesSource = nullableRefTypesTree.ToString();
      Assert.Contains("public sealed class NullableRefTypesClassSerializer : ISerializer<global::TestNamespace.NullableRefTypesClass?>", nullableRefTypesSource);
      Assert.Contains("SerializerRegistry<string>.GetWrite()(ref writer, value.NonNullableAddress);", nullableRefTypesSource);
      Assert.Contains("SerializerRegistry<string?>.GetWrite()(ref writer, value.NullableToken);", nullableRefTypesSource);
      Assert.Contains("SerializerRegistry<string>.GetTryRead()(ref reader, out var member_NonNullableAddress)", nullableRefTypesSource);
      Assert.Contains("SerializerRegistry<string?>.GetTryRead()(ref reader, out var member_NullableToken)", nullableRefTypesSource);
      Assert.Contains("SerializerRegistry<string>.GetCalculateByteLength()(value.NonNullableAddress)", nullableRefTypesSource);
      Assert.Contains("SerializerRegistry<string?>.GetCalculateByteLength()(value.NullableToken)", nullableRefTypesSource);
   }
}
