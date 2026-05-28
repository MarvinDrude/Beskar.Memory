using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

[Generator]
public sealed partial class TypeIdGenerator : IIncrementalGenerator
{
   public const string GeneratorName = "TypeIdGenerator";
   public const string GeneratorVersion = "1.5.3";

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var assemblyNameProvider = context.CompilationProvider
         .Select(static (c, _) => c.AssemblyName?
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty)
            .Trim() ?? "UnknownAssembly");

      var maybeSpecProvider = context.SyntaxProvider
         .ForAttributeWithMetadataName(
            AttributeTypeIdFullName,
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
