using Beskar.Memory.Code.Models.Symbols.Archetypes;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator.Models;

public sealed record UnionSpec(
   int Tag, NamedTypeSymbolArchetype Type);
