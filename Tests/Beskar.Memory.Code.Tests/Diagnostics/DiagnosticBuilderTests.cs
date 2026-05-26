using System;
using Xunit;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Tests.Diagnostics
{
   public class DummySpec
   {
      public string Name { get; set; } = "Dummy";
   }

   public class DiagnosticBuilderTests
   {
      [Fact]
      public void Create_And_Add_BuildsFailedMaybeSpecWithAddedDiagnostics()
      {
         using var builder = DiagnosticBuilder<DummySpec>.Create();
         builder.Add("ERR001", "Arg1", "Arg2");
         builder.Add("ERR002");

         var result = builder.Build();

         Assert.False(result.HasValue);
         Assert.NotNull(result.Value);
         Assert.Equal(2, result.Diagnostics.Count);

         Assert.Equal("ERR001", result.Diagnostics[0].DiagnosticId);
         Assert.Equal(2, result.Diagnostics[0].Arguments.Count);
         Assert.Equal("Arg1", result.Diagnostics[0].Arguments[0]);
         Assert.Equal("Arg2", result.Diagnostics[0].Arguments[1]);

         Assert.Equal("ERR002", result.Diagnostics[1].DiagnosticId);
         Assert.Empty(result.Diagnostics[1].Arguments);
      }

      [Fact]
      public void Build_WithValue_ReturnsSuccessfulMaybeSpecWithDiagnostics()
      {
         using var builder = DiagnosticBuilder<DummySpec>.Create();
         builder.Add("WARN001", "WarningArg");

         var dummy = new DummySpec { Name = "Custom" };
         var result = builder.Build(dummy);

         Assert.True(result.HasValue);
         Assert.Same(dummy, result.Value);
         Assert.Single(result.Diagnostics);
         Assert.Equal("WARN001", result.Diagnostics[0].DiagnosticId);
         Assert.Equal("WarningArg", result.Diagnostics[0].Arguments[0]);
      }

      [Fact]
      public void CreateSingle_ReturnsFailedMaybeSpecWithOneDiagnostic()
      {
         var result = DiagnosticBuilder<DummySpec>.CreateSingle("ERR999", "SingleArg");

         Assert.False(result.HasValue);
         Assert.Single(result.Diagnostics);
         Assert.Equal("ERR999", result.Diagnostics[0].DiagnosticId);
         Assert.Equal("SingleArg", result.Diagnostics[0].Arguments[0]);
      }

      [Fact]
      public void CreateEmpty_ReturnsFailedMaybeSpecWithNoDiagnostics()
      {
         var result = DiagnosticBuilder<DummySpec>.CreateEmpty();

         Assert.False(result.HasValue);
         Assert.Empty(result.Diagnostics);
      }

      [Fact]
      public void Build_AfterDispose_ThrowsInvalidOperationException()
      {
         var builder = DiagnosticBuilder<DummySpec>.Create();
         builder.Dispose();

         Assert.Throws<InvalidOperationException>(() => builder.Build());
         Assert.Throws<InvalidOperationException>(() => builder.Build(new DummySpec()));
      }

      [Fact]
      public void DiagnosticSpec_RecordEqualityWorks()
      {
         SequenceArray<string> args1 = ["A", "B"];
         SequenceArray<string> args2 = ["A", "B"];
         var spec1 = new DiagnosticSpec("ID", args1);
         var spec2 = new DiagnosticSpec("ID", args2);

         Assert.Equal(spec1, spec2);
      }
   }
}
