using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.Tests.Helpers
{
   public static class TestCompilationHelper
   {
      private static readonly Lazy<List<MetadataReference>> ReferencesLazy = new Lazy<List<MetadataReference>>(() =>
      {
         var references = new List<MetadataReference>();

         // Load standard .NET 10.0 reference assemblies
         foreach (var reference in Basic.Reference.Assemblies.Net100.References.All)
         {
            references.Add(reference);
         }

         // Load Beskar.Memory core assembly
         var memoryAssemblyLocation = typeof(Beskar.Memory.Pools.ObjectPool<>).Assembly.Location;
         if (!string.IsNullOrEmpty(memoryAssemblyLocation))
         {
            references.Add(MetadataReference.CreateFromFile(memoryAssemblyLocation));
         }

         // Load Beskar.Memory.Code assembly
         var codeAssemblyLocation = typeof(Beskar.Memory.Code.Models.Diagnostics.DiagnosticSpec).Assembly.Location;
         if (!string.IsNullOrEmpty(codeAssemblyLocation))
         {
            references.Add(MetadataReference.CreateFromFile(codeAssemblyLocation));
         }

         return references;
      });

      public static CSharpCompilation Compile(string sourceCode)
      {
         var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
         var compilation = CSharpCompilation.Create(
            assemblyName: $"TestAssembly_{Guid.NewGuid():N}",
            syntaxTrees: new[] { syntaxTree },
            references: ReferencesLazy.Value,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

         // Verify that compilation is clean (or has only minor issues)
         var diagnostics = compilation.GetDiagnostics();
         var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
         if (errors.Any())
         {
            throw new InvalidOperationException(
               $"Compilation failed with errors:\n{string.Join("\n", errors.Select(e => e.GetMessage()))}");
         }

         return compilation;
      }

      public static INamedTypeSymbol GetNamedTypeSymbol(string sourceCode, string typeName)
      {
         var compilation = Compile(sourceCode);
         var symbol = compilation.GetTypeByMetadataName(typeName);
         if (symbol == null)
         {
            throw new InvalidOperationException($"Type symbol '{typeName}' not found in compilation.");
         }
         return symbol;
      }

      public static AttributeData GetAttributeData(string sourceCode, string typeName, int attributeIndex = 0)
      {
         var typeSymbol = GetNamedTypeSymbol(sourceCode, typeName);
         var attributes = typeSymbol.GetAttributes();
         if (attributes.Length <= attributeIndex)
         {
            throw new InvalidOperationException(
               $"Type symbol '{typeName}' does not have an attribute at index {attributeIndex}. Found {attributes.Length} attributes.");
         }
         return attributes[attributeIndex];
      }
   }
}
