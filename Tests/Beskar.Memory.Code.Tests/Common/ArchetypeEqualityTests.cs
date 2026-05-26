using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Code.Tests.Helpers;

namespace Beskar.Memory.Code.Tests.Common
{
   public class ArchetypeEqualityTests
   {
      private const string Source1 = @"
public class Item
{
   public string Code { get; set; }
   public int Value;
   public void Work() {}
}
";

      private const string Source2 = @"
public class Item
{
   public string Code { get; set; }
   public int Value;
   public void Work() {}
}
";

      private const string SourceDifferent = @"
public class ItemDifferent
{
   public string Code { get; set; }
   public int Value;
   private void Work() {}
}
";

      [Fact]
      public void NamedTypeSymbolSpec_ValueBasedEqualityWorks()
      {
         var typeSymbolA = TestCompilationHelper.GetNamedTypeSymbol(Source1, "Item");
         var typeSymbolB = TestCompilationHelper.GetNamedTypeSymbol(Source2, "Item");
         var typeSymbolDiff = TestCompilationHelper.GetNamedTypeSymbol(SourceDifferent, "ItemDifferent");

         var options = new ArchetypeTransformOptions { NamedTypes = NamedTypeTransformOptions.Full };

         var specA = NamedTypeSymbolSpecTransformer.Transform(typeSymbolA, options: options);
         var specB = NamedTypeSymbolSpecTransformer.Transform(typeSymbolB, options: options);
         var specDiff = NamedTypeSymbolSpecTransformer.Transform(typeSymbolDiff, options: options);

         // Assert equal for identical structures in different compilations
         Assert.Equal(specA, specB);
         Assert.Equal(specA.GetHashCode(), specB.GetHashCode());

         // Assert not equal for different structures
         Assert.NotEqual(specA, specDiff);
      }

      [Fact]
      public void MethodSymbolArchetype_ValueBasedEqualityWorks()
      {
         var typeSymbolA = TestCompilationHelper.GetNamedTypeSymbol(Source1, "Item");
         var typeSymbolB = TestCompilationHelper.GetNamedTypeSymbol(Source2, "Item");
         var typeSymbolDiff = TestCompilationHelper.GetNamedTypeSymbol(SourceDifferent, "ItemDifferent");

         var methodA = typeSymbolA.GetMembers("Work").OfType<IMethodSymbol>().First();
         var methodB = typeSymbolB.GetMembers("Work").OfType<IMethodSymbol>().First();
         var methodDiff = typeSymbolDiff.GetMembers("Work").OfType<IMethodSymbol>().First();

         var options = new ArchetypeTransformOptions();

         var archA = MethodSymbolArchetypeTransformer.Transform(methodA, options: options);
         var archB = MethodSymbolArchetypeTransformer.Transform(methodB, options: options);
         var archDiff = MethodSymbolArchetypeTransformer.Transform(methodDiff, options: options);

         Assert.Equal(archA, archB);
         Assert.Equal(archA.GetHashCode(), archB.GetHashCode());
         Assert.NotEqual(archA, archDiff);
      }

      [Fact]
      public void FieldSymbolArchetype_ValueBasedEqualityWorks()
      {
         var typeSymbolA = TestCompilationHelper.GetNamedTypeSymbol(Source1, "Item");
         var typeSymbolB = TestCompilationHelper.GetNamedTypeSymbol(Source2, "Item");

         var fieldA = typeSymbolA.GetMembers("Value").OfType<IFieldSymbol>().First();
         var fieldB = typeSymbolB.GetMembers("Value").OfType<IFieldSymbol>().First();

         var options = new ArchetypeTransformOptions();

         var archA = FieldSymbolArchetypeTransformer.Transform(fieldA, options: options);
         var archB = FieldSymbolArchetypeTransformer.Transform(fieldB, options: options);

         Assert.Equal(archA, archB);
         Assert.Equal(archA.GetHashCode(), archB.GetHashCode());
      }

      [Fact]
      public void PropertySymbolArchetype_ValueBasedEqualityWorks()
      {
         var typeSymbolA = TestCompilationHelper.GetNamedTypeSymbol(Source1, "Item");
         var typeSymbolB = TestCompilationHelper.GetNamedTypeSymbol(Source2, "Item");

         var propA = typeSymbolA.GetMembers("Code").OfType<IPropertySymbol>().First();
         var propB = typeSymbolB.GetMembers("Code").OfType<IPropertySymbol>().First();

         var options = new ArchetypeTransformOptions();

         var archA = PropertySymbolArchetypeTransformer.Transform(propA, options: options);
         var archB = PropertySymbolArchetypeTransformer.Transform(propB, options: options);

         Assert.Equal(archA, archB);
         Assert.Equal(archA.GetHashCode(), archB.GetHashCode());
      }
   }
}
