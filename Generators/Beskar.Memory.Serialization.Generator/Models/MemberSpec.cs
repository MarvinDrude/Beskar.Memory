using Beskar.Memory.Code.Models.Symbols.Archetypes;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator.Models;

public sealed record MemberSpec(
   string Name, bool IsProperty, int Order,
   NamedTypeSymbolArchetype? SerializerType);
