using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.EnumGenerator.Generator;

[Generator]
public sealed partial class EnumGenerator : IIncrementalGenerator
{
   public const string GeneratorName = "EnumGenerator";
   public const string GeneratorVersion = "1.6.1";

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var assemblyNameProvider = context.CompilationProvider
         .Select(static (c, _) => c.AssemblyName?
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty)
            .Trim() ?? "UnknownAssembly");

      var maybeSpecProvider = context.SyntaxProvider
         .ForAttributeWithMetadataName(
            FastEnumAttributeFullName,
            predicate: static (_, _) => true,
            transform: Transform);

      var combined = maybeSpecProvider
         .Combine(assemblyNameProvider);

      context.RegisterSourceOutput(combined, static (ctx, source)
         => Render(ctx, source.Right, source.Left));

      context.RegisterPostInitializationOutput(static ctx =>
      {
         ctx.AddSource($"{GeneratorName}.g.cs", $"// Version {GeneratorVersion}");
      });
   }
}
