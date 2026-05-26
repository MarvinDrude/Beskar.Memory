using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

public sealed partial class TypeIdGenerator
{
   private static string InvalidTargetDiagnosticId => InvalidTargetRule.Id;
   private static readonly DiagnosticDescriptor InvalidTargetRule = new (
      id: "TIG001",
      title: "Invalid [TypeSafeId] usage",
      messageFormat: "Invalid struct for being decorated with [TypeSafeId]. Needs to be readonly partial record struct with one unmanaged Parameter 'Value'.",
      category: "TypeIdGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static readonly Dictionary<string, DiagnosticDescriptor> Diagnostics = new()
   {
      [InvalidTargetDiagnosticId] = InvalidTargetRule
   };
}