using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

/// <summary>
/// Represents a field symbol archetype, pairing general symbol details with field-specific specifications.
/// </summary>
/// <param name="Symbol">The general symbol specifications.</param>
/// <param name="Field">The field-specific specifications.</param>
[DebuggerDisplay("Field: {Symbol.FullName, nq}")]
public readonly record struct FieldSymbolArchetype(
   SymbolSpec Symbol,
   FieldSymbolSpec Field);
