using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Beskar.Memory.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.TypeIdGenerator.Tests.Common;

public sealed class TestCompilationCreator
{
   private readonly List<SourceFile> _sourceTexts = [];
   private readonly List<IIncrementalGenerator> _sourcesGenerators = [];
   private readonly List<MetadataReference> _references = [];

   private LanguageVersion _languageVersionSyntaxTrees = LanguageVersion.Preview; 
   private int _fileNameIncrement = 1;

   private bool _enableNullable = true;
   private string _assemblyName = "TestAssembly";
   private CSharpCompilationOptions _compOptions = new (OutputKind.DynamicallyLinkedLibrary);
   
   public TestCompilationResult Create(CancellationToken ct = default)
   {
      _compOptions = _compOptions.WithNullableContextOptions(_enableNullable 
         ? NullableContextOptions.Enable : NullableContextOptions.Disable);
      
      var syntaxTreeOptions = new CSharpParseOptions(
         _languageVersionSyntaxTrees, DocumentationMode.Diagnose);
      var syntaxTrees = _sourceTexts.Select(t => CSharpSyntaxTree.ParseText(
         t.SourceText, syntaxTreeOptions, t.FileName, cancellationToken: ct));
      
      IEnumerable<MetadataReference> references = [.. _references];
      
      Compilation compilation = CSharpCompilation.Create(
         _assemblyName, syntaxTrees, references, _compOptions);
      
      ImmutableArray<Diagnostic> generatorDiagnostics = [];
      ImmutableArray<SyntaxTree> generatedSyntaxTrees = [];
      
      if (_sourcesGenerators.Count > 0)
      {
         GeneratorDriver driver = CSharpGeneratorDriver.Create(
            _sourcesGenerators.Select(g => g.AsSourceGenerator()).ToArray(),
            parseOptions: syntaxTreeOptions);
         
         driver = driver.RunGenerators(compilation, cancellationToken: ct);
         
         var genResult = driver.GetRunResult();
         generatedSyntaxTrees = genResult.GeneratedTrees;
         generatorDiagnostics = genResult.Diagnostics;
         
         compilation = compilation.AddSyntaxTrees(generatedSyntaxTrees);
      }
      
      return new TestCompilationResult()
      {
         Compilation = compilation,
         GeneratedDiagnostics = generatorDiagnostics,
         Diagnostics = compilation.GetDiagnostics(ct),
         GeneratedSyntaxTrees = generatedSyntaxTrees
      };
   }

   public TestCompilationCreator AddSourceText(string text, string fileName)
   {
      _sourceTexts.Add(new  SourceFile(text, fileName));
      return this;
   }
   
   public TestCompilationCreator AddSourceText(string text)
   {
      _sourceTexts.Add(new  SourceFile(text, $"Source{_fileNameIncrement++}.cs"));
      return this;
   }

   public TestCompilationCreator AddSourceGenerator<T>(T generator)
      where T : IIncrementalGenerator
   {
      _sourcesGenerators.Add(generator);
      return this;
   }

   public TestCompilationCreator WithSyntaxTreeLangVersion(LanguageVersion languageVersionSyntaxTrees)
   {
      _languageVersionSyntaxTrees = languageVersionSyntaxTrees;
      return this;
   }

   public TestCompilationCreator WithAssemblyName(string assemblyName)
   {
      _assemblyName = assemblyName;
      return this;
   }

   public TestCompilationCreator WithCompilationOptions(CSharpCompilationOptions compilationOptions)
   {
      _compOptions = compilationOptions;
      return this;
   }

   public TestCompilationCreator WithReferences(IEnumerable<MetadataReference> references)
   {
      _references.AddRange(references);
      return this;
   }

   public TestCompilationCreator WithReferenceByType<T>()
   {
      var location = typeof(T).Assembly.Location;
      _references.Add(MetadataReference.CreateFromFile(location));
      
      return this;
   }
   
   public TestCompilationCreator SuppressDiagnostic(string diagnosticId)
   {
      var currentOptions = _compOptions.SpecificDiagnosticOptions;
      var updatedOptions = currentOptions.Add(diagnosticId, ReportDiagnostic.Suppress);

      _compOptions = _compOptions.WithSpecificDiagnosticOptions(updatedOptions);
      return this;
   }
   
   public TestCompilationCreator SuppressDiagnostics(params string[] diagnosticIds)
   {
      foreach (var id in diagnosticIds)
      {
         SuppressDiagnostic(id);
      }
      
      return this;
   }
   
   public TestCompilationCreator EnableNullable(bool enableNullable)
   {
      _enableNullable = enableNullable;
      return this;
   }
   
   public TestCompilationCreator AddTestScenario(string relativePath)
   {
      var projectRoot = GetProjectRoot();
      var fullPath = Path.Combine(projectRoot, relativePath);

      if (!Directory.Exists(fullPath))
      {
         throw new DirectoryNotFoundException($"The scenario folder was not found at: {fullPath}");
      }

      var files = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories);
      foreach (var file in files)
      {
         var content = File.ReadAllText(file);
         var fileName = Path.GetRelativePath(projectRoot, file);
         
         _sourceTexts.Add(new SourceFile(content, fileName));
      }

      return this;
   }
   
   private string GetProjectRoot()
   {
      var directory = new DirectoryInfo(AppContext.BaseDirectory);
      while (directory != null && !directory.GetFiles("*.csproj").Any())
      {
         directory = directory.Parent;
      }

      return directory?.FullName ?? throw new DirectoryNotFoundException("Could not find the project root.");
   }

   private sealed record SourceFile(string SourceText, string FileName);
}

public sealed class TestCompilationResult
{
   public required Compilation Compilation { get; set; }
   
   public ImmutableArray<Diagnostic> Diagnostics { get; set; }
   public ImmutableArray<Diagnostic> GeneratedDiagnostics { get; set; }

   public ImmutableArray<SyntaxTree> GeneratedSyntaxTrees { get; set; } = [];

   public Diagnostic[] Errors => field ??= GetDiagnostics(DiagnosticSeverity.Error).ToArray();
   public Diagnostic[] GeneratedErrors => field ??= GetGeneratedDiagnostics(DiagnosticSeverity.Error).ToArray();
   
   public IEnumerable<Diagnostic> GetDiagnostics(DiagnosticSeverity severity)
      => Diagnostics.Where(d => d.Severity == severity);

   public IEnumerable<Diagnostic> GetGeneratedDiagnostics(DiagnosticSeverity severity)
      => GeneratedDiagnostics.Where(d => d.Severity == severity);

   public string GetDebugReport()
   {
      var writer = new CodeTextWriter(
         stackalloc char[512], stackalloc char[128]);

      try
      {
         var allDiagnostics = Diagnostics.Concat(GeneratedDiagnostics).ToArray();
         
         if (allDiagnostics.Length == 0)
         {
            return "No diagnostics";
         }

         foreach (var diagnostic in allDiagnostics)
         {
            var location = diagnostic.Location;
            var lineSpan = location.GetLineSpan();
            
            var fileName = lineSpan.Path ?? "Unknown";
            var startLine = lineSpan.StartLinePosition.Line;
            var startChar = lineSpan.StartLinePosition.Character;
            
            writer.WriteLineInterpolated($"[{diagnostic.Id}] {diagnostic.GetMessage()} ({fileName}:{startLine}:{startChar})");

            if (location.SourceTree is { } tree)
            {
               var sourceText = location.SourceTree.GetText();
               var lineContent = sourceText.Lines[startLine].ToString();
               
               writer.WriteLine($"Source:");
               writer.WriteLineInterpolated($" > {lineContent}");
            }

            writer.WriteLine(new string('-', 40));
         }
         
         return writer.WrittenSpan.ToString();
      }
      finally
      {
         writer.Dispose();
      }
   }
}
