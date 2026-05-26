using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.ObserveGenerator.Generator.Models;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator;

public sealed partial class ObserveGenerator
{
   private static MaybeSpec<ObserveSpec> Transform(
      GeneratorAttributeSyntaxContext context,
      CancellationToken ct)
   {
      ct.ThrowIfCancellationRequested();

      var symbol = (INamedTypeSymbol)context.TargetSymbol;
      var attributes = symbol.GetAttributes();

      var metricSpec = GetMeterAttribute(symbol, attributes);
      var activitySpec = GetActivityAttribute(symbol, attributes);
      var instrumentSpecs = GetInstrumentAttributes(symbol, attributes);
      
      ct.ThrowIfCancellationRequested();

      if (metricSpec is null && activitySpec is null)
      {
         return DiagnosticBuilder<ObserveSpec>.CreateSingle(InvalidTargetDiagnosticId);
      }

      using var builder = DiagnosticBuilder<ObserveSpec>.Create(8);
      var namedInfo = symbol.CreateNamedArchetype(CreateTransformOptions());
      
      return builder.Build(new ObserveSpec(
         namedInfo,
         activitySpec,
         metricSpec,
         new SequenceArray<ObserveInstrumentSpec>(instrumentSpecs)));
   }
   
   private static ArchetypeTransformOptions CreateTransformOptions()
   {
      var options = new ArchetypeTransformOptions
      {
         NamedTypes =
         {
            Load = new NamedTypeSymbolLoadFlags()
            {
               TypeParameters = true
            }
         }
      };

      return options;
   }
}
