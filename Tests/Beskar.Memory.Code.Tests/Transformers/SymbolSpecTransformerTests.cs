using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Code.Tests.Helpers;

namespace Beskar.Memory.Code.Tests.Transformers
{
   public class SymbolSpecTransformerTests
   {
      private const string Source = @"
public class SampleClass
      {
         public string Text { get; set; }
         public int Number;
         public void DoWork() {}
         private void DoSecret() {}
      }
";

      [Fact]
      public void Transform_MinimalOptions_DoesNotLoadMembers()
      {
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(Source, "SampleClass");
         var options = new ArchetypeTransformOptions
         {
            NamedTypes = NamedTypeTransformOptions.Minimal
         };

         var spec = NamedTypeSymbolSpecTransformer.Transform(classSymbol, options: options);

         Assert.NotNull(spec);
         Assert.False(spec.IsEnum);
         Assert.Equal(0, spec.Arity);
         Assert.Throws<InvalidOperationException>(() => spec.Methods);
         Assert.Throws<InvalidOperationException>(() => spec.Properties);
         Assert.Throws<InvalidOperationException>(() => spec.Fields);
      }

      [Fact]
      public void Transform_FullOptions_LoadsAllMembers()
      {
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(Source, "SampleClass");
         var options = new ArchetypeTransformOptions
         {
            NamedTypes = NamedTypeTransformOptions.Full
         };

         var spec = NamedTypeSymbolSpecTransformer.Transform(classSymbol, options: options);

         Assert.NotNull(spec);
         
         // 2 custom methods + constructors/getters/setters (Wait, methods might contain constructor and property accessors!)
         // But the class members of type MethodSymbol will contain: DoWork, DoSecret, .ctor, get_Text, set_Text
         // Let's assert on the presence of our actual methods
         Assert.NotEmpty(spec.Methods);
         var methodNames = spec.Methods.Select(m => m.Symbol.Name).ToList();
         Assert.Contains("DoWork", methodNames);
         Assert.Contains("DoSecret", methodNames);

         // Field symbol: Number (backing field for auto-property Text might also be present depending on compiler, but it's internal/private or generated)
         // Our explicit field is "Number"
         Assert.NotEmpty(spec.Fields);
         Assert.Contains("Number", spec.Fields.Select(f => f.Symbol.Name));

         // Property symbol: Text
         Assert.Single(spec.Properties);
         Assert.Equal("Text", spec.Properties[0].Symbol.Name);
      }

      [Fact]
      public void Transform_FilteredOptions_FiltersMembersCorrectly()
      {
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(Source, "SampleClass");
         
         var namedTypeOptions = NamedTypeTransformOptions.Full;
         // Filter: only include public methods
         namedTypeOptions.MethodFilter = m => m.DeclaredAccessibility == Accessibility.Public;

         var options = new ArchetypeTransformOptions
         {
            NamedTypes = namedTypeOptions
         };

         var spec = NamedTypeSymbolSpecTransformer.Transform(classSymbol, options: options);

         Assert.NotNull(spec);
         Assert.NotEmpty(spec.Methods);

         var methodNames = spec.Methods.Select(m => m.Symbol.Name).ToList();
         Assert.Contains("DoWork", methodNames);
         Assert.DoesNotContain("DoSecret", methodNames); // private, should be filtered out
      }
   }
}
