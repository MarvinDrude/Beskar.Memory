using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

/// <summary>
/// Represents a type parameter symbol archetype, pairing general symbol details with type parameter-specific specifications.
/// </summary>
/// <param name="Symbol">The general symbol specifications.</param>
/// <param name="TypeParameter">The type parameter-specific specifications.</param>
[DebuggerDisplay("TypeParameter: {Symbol.FullName, nq}")]
public readonly record struct TypeParameterArchetype(
   SymbolSpec Symbol,
   TypeParameterSymbolSpec TypeParameter);
