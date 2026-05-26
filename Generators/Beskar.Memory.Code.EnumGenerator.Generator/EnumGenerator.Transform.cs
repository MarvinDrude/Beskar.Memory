using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.EnumGenerator.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.EnumGenerator.Generator;

public sealed partial class EnumGenerator
{
   private static MaybeSpec<FastEnumSpec> Transform(
      GeneratorAttributeSyntaxContext context,
      CancellationToken ct)
   {
      ct.ThrowIfCancellationRequested();

      var symbol = (INamedTypeSymbol)context.TargetSymbol;
      var attributes = symbol.GetAttributes();

      if (GetFastEnumAttribute(attributes) is not { } attribute)
      {
         return DiagnosticBuilder<FastEnumSpec>.CreateEmpty();
      }
      
      ct.ThrowIfCancellationRequested();
      using var builder = DiagnosticBuilder<FastEnumSpec>.Create(8);

      var namedType = symbol.CreateNamedArchetype(CreateOptions());
      return builder.Build(new FastEnumSpec(namedType));
   }

   private static ArchetypeTransformOptions CreateOptions()
   {
      var opts = new ArchetypeTransformOptions()
      {
         NamedTypes =
         {
            Depth = 2,
            FieldFilter = static (field) => field.HasConstantValue,
            Load = new NamedTypeSymbolLoadFlags()
            {
               Fields = true
            }
         }
      };

      return opts;
   }
}
