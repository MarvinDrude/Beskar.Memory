using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

public sealed partial class SerializationGenerator
{
   private static string InvalidTargetDiagnosticId => InvalidTargetRule.Id;
   private static readonly DiagnosticDescriptor InvalidTargetRule = new (
      id: "SG001",
      title: "Invalid serialization target",
      messageFormat: "Invalid class or struct decorated with [BeskarObject]. Needs to be partial.",
      category: "SerializationGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static string DuplicateOrderDiagnosticId => DuplicateOrderRule.Id;
   private static readonly DiagnosticDescriptor DuplicateOrderRule = new (
      id: "SG002",
      title: "Duplicate BeskarOrder index",
      messageFormat: "Type '{0}' has duplicate [BeskarOrder] index '{1}' on member '{2}'",
      category: "SerializationGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static string MissingConstructorDiagnosticId => MissingConstructorRule.Id;
   private static readonly DiagnosticDescriptor MissingConstructorRule = new (
      id: "SG003",
      title: "Missing matching constructor",
      messageFormat: "Type '{0}' has no parameterless constructor and no parameterized constructor matching the non-ignored ordered members",
      category: "SerializationGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static string InvalidSerializerOverrideDiagnosticId => InvalidSerializerOverrideRule.Id;
   private static readonly DiagnosticDescriptor InvalidSerializerOverrideRule = new (
      id: "SG004",
      title: "Invalid serializer override",
      messageFormat: "Custom serializer '{0}' specified on member '{1}' does not implement ISerializer<{2}>",
      category: "SerializationGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static string MissingOrderAttributeDiagnosticId => MissingOrderAttributeRule.Id;
   private static readonly DiagnosticDescriptor MissingOrderAttributeRule = new (
      id: "SG005",
      title: "Missing BeskarOrder attribute",
      messageFormat: "Public instance member '{0}' in '{1}' must either be decorated with [BeskarIgnore] or have a [BeskarOrder] attribute",
      category: "SerializationGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static readonly Dictionary<string, DiagnosticDescriptor> Diagnostics = new()
   {
      [InvalidTargetDiagnosticId] = InvalidTargetRule,
      [DuplicateOrderDiagnosticId] = DuplicateOrderRule,
      [MissingConstructorDiagnosticId] = MissingConstructorRule,
      [InvalidSerializerOverrideDiagnosticId] = InvalidSerializerOverrideRule,
      [MissingOrderAttributeDiagnosticId] = MissingOrderAttributeRule
   };
}
