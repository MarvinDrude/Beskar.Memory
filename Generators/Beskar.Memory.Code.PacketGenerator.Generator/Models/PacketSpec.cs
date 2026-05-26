using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.PacketGenerator.Generator.Models;

public readonly record struct PacketSpec(
   SequenceArray<string> RegistryFullTypeNames,
   NamedTypeSymbolArchetype NamedTypeArchetype);
