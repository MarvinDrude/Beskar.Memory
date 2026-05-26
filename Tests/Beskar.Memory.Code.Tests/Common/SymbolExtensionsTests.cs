using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Beskar.Memory.Code.Common.Enums;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Tests.Helpers;

namespace Beskar.Memory.Code.Tests.Common
{
   public class SymbolExtensionsTests
   {
      [Theory]
      [InlineData(Accessibility.Public, "public")]
      [InlineData(Accessibility.Private, "private")]
      [InlineData(Accessibility.Protected, "protected")]
      [InlineData(Accessibility.Internal, "internal")]
      [InlineData(Accessibility.ProtectedOrInternal, "protected internal")]
      [InlineData(Accessibility.NotApplicable, "private")]
      public void Accessibility_ToKeywordString_ReturnsCorrectKeyword(Accessibility accessibility, string expectedKeyword)
      {
         Assert.Equal(expectedKeyword, accessibility.ToKeywordString());
      }

      [Fact]
      public void ExplicitOrImplicitInterfaceImplementations_ImplicitImplementation_ReturnsInterfaceMethod()
      {
         const string source = @"
public interface ITestService
{
   void Execute();
}

public class TestService : ITestService
{
   public void Execute() {}
}
";
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(source, "TestService");
         var executeMethodSymbol = classSymbol.GetMembers("Execute").FirstOrDefault();

         Assert.NotNull(executeMethodSymbol);

         var implementations = executeMethodSymbol.ExplicitOrImplicitInterfaceImplementations();

         Assert.Single(implementations);
         Assert.Equal("Execute", implementations[0].Name);
         Assert.Equal("ITestService", implementations[0].ContainingType.Name);
      }

      [Fact]
      public void ExplicitOrImplicitInterfaceImplementations_ExplicitImplementation_ReturnsInterfaceMethod()
      {
         const string source = @"
public interface ITestService
{
   void Execute();
}

public class TestService : ITestService
{
   void ITestService.Execute() {}
}
";
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(source, "TestService");
         
         // Explicit method implementation symbol names in Roslyn contain the fully qualified interface name
         var executeMethodSymbol = classSymbol.GetMembers().FirstOrDefault(m => m.Name.EndsWith("Execute"));

         Assert.NotNull(executeMethodSymbol);

         var implementations = executeMethodSymbol.ExplicitOrImplicitInterfaceImplementations();

         Assert.Single(implementations);
         Assert.Equal("Execute", implementations[0].Name);
         Assert.Equal("ITestService", implementations[0].ContainingType.Name);
      }
   }
}
