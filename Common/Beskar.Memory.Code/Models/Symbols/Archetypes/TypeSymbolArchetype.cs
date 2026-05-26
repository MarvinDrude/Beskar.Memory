using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

/// <summary>
/// Represents a type symbol archetype, pairing general symbol details, type-specific specifications, and optional named type specifications.
/// </summary>
/// <param name="Symbol">The general symbol specifications.</param>
/// <param name="Type">The type symbol specifications.</param>
/// <param name="NamedType">The optional named type-specific specifications.</param>
[DebuggerDisplay("Type: {Symbol.FullName, nq}")]
public readonly record struct TypeSymbolArchetype(
   SymbolSpec Symbol,
   TypeSymbolSpec Type,
   NamedTypeSymbolSpec? NamedType);
