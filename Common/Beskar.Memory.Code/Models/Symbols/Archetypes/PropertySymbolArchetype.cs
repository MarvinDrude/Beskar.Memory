using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

/// <summary>
/// Represents a property symbol archetype, pairing general symbol details with property-specific specifications.
/// </summary>
/// <param name="Symbol">The general symbol specifications.</param>
/// <param name="Property">The property-specific specifications.</param>
[DebuggerDisplay("Property: {Symbol.FullName, nq}")]
public readonly record struct PropertySymbolArchetype(
   SymbolSpec Symbol,
   PropertySymbolSpec Property);
