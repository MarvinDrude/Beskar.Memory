using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

[Generator]
public sealed partial class SerializationGenerator : IIncrementalGenerator
{
   public const string GeneratorName = "SerializationGenerator";
   public const string GeneratorVersion = "1.5.1";

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {

   }
}
