using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator;

[Generator]
public sealed partial class ObserveGenerator : IIncrementalGenerator
{
   public const string GeneratorName = "ObserveGenerator";
   public const string GeneratorVersion = "1.5.1";
   
   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var assemblyNameProvider = context.CompilationProvider
         .Select(static (c, _) => c.AssemblyName?
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty)
            .Replace("-", string.Empty)
            .Trim() ?? "UnknownAssembly");
      
      var maybeSpecProvider = context.SyntaxProvider
         .ForAttributeWithMetadataName(
            ObserveAttributeFullName,
            predicate: static (_, _) => true,
            transform: Transform);

      var combined = maybeSpecProvider
         .Combine(assemblyNameProvider);
      
      var collected = assemblyNameProvider
         .Combine(maybeSpecProvider
            .Where(static x => x.HasValue)
            .Select(static (x, _) => x.Value)
            .Collect()
         );
      
      context.RegisterSourceOutput(combined, static (ctx, source) 
         => Render(ctx, source.Right, source.Left));
      
      context.RegisterSourceOutput(collected, static (ctx, source) 
         => RenderExtensions(ctx, source.Left, source.Right));
      
      context.RegisterPostInitializationOutput(static ctx =>
      {
         ctx.AddSource($"{GeneratorName}.g.cs", $"// Version {GeneratorVersion}");
      });
   }
}
