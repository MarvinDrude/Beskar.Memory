using Beskar.Memory.Code.Models.Symbols.Archetypes;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator.Models;

public readonly record struct TypeSafeIdSpec(
   TypeSafeIdAttributeSpec AttributeSpec,
   NamedTypeSymbolArchetype NamedTargetArchetype);