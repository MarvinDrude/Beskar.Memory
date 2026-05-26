using Beskar.Memory.Code.Models.Symbols.Archetypes;

namespace Beskar.Memory.Code.PacketGenerator.Generator.Models;

public readonly record struct PacketRegistrySpec(
   NamedTypeSymbolArchetype NamedTypeArchetype,
   string? StateTypeFullName);
