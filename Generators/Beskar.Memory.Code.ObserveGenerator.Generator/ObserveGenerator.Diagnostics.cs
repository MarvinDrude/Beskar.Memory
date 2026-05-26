using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator;

public sealed partial class ObserveGenerator
{
   private static string InvalidTargetDiagnosticId => InvalidTargetRule.Id;
   private static readonly DiagnosticDescriptor InvalidTargetRule = new (
      id: "OBG001",
      title: "Invalid ObserveGenerator Target usage",
      messageFormat: "Invalid configuration for ObserveGenerator. At least one of ObserveActivity or ObserveMeter must be specified.",
      category: "ObserveGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static readonly Dictionary<string, DiagnosticDescriptor> Diagnostics = new()
   {
      [InvalidTargetDiagnosticId] = InvalidTargetRule
   };
}
