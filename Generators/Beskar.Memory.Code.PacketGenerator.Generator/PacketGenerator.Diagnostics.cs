using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.PacketGenerator.Generator;

public sealed partial class PacketGenerator
{
   private static string InvalidTargetDiagnosticId => InvalidTargetRule.Id;
   private static readonly DiagnosticDescriptor InvalidTargetRule = new (
      id: "PG001",
      title: "Invalid [Packet] usage",
      messageFormat: "Packet needs to be class or struct with interface IPacket",
      category: "PacketGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);
   
   private static string InvalidRegistryTargetDiagnosticId => InvalidRegistryTargetRule.Id;
   private static readonly DiagnosticDescriptor InvalidRegistryTargetRule = new (
      id: "PG002",
      title: "Invalid [PacketRegistry] usage",
      messageFormat: "Invalid type: PacketRegistry is required to be a class and must derive from BasePacketRegistry",
      category: "PacketGenerator",
      defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true);

   private static readonly Dictionary<string, DiagnosticDescriptor> Diagnostics = new()
   {
      [InvalidTargetDiagnosticId] = InvalidTargetRule,
      [InvalidRegistryTargetDiagnosticId] = InvalidRegistryTargetRule,
   };
}
