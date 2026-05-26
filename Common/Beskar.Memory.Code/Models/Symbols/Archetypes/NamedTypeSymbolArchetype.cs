using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

/// <summary>
/// Represents a named type symbol archetype, combining general symbol details, type specifications, and named type details.
/// </summary>
/// <param name="Symbol">The general symbol specifications.</param>
/// <param name="Type">The type symbol specifications.</param>
/// <param name="NamedType">The named type-specific specifications.</param>
[DebuggerDisplay("NamedType: {Symbol.FullName, nq}")]
public readonly record struct NamedTypeSymbolArchetype(
   SymbolSpec Symbol,
   TypeSymbolSpec Type,
   NamedTypeSymbolSpec NamedType);
