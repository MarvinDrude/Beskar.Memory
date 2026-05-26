using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

/// <summary>
/// Represents a parameter symbol archetype, pairing general symbol details with parameter-specific specifications.
/// </summary>
/// <param name="Symbol">The general symbol specifications.</param>
/// <param name="Parameter">The parameter-specific specifications.</param>
[DebuggerDisplay("Parameter: {Symbol.FullName, nq}")]
public readonly record struct ParameterSymbolArchetype(
   SymbolSpec Symbol,
   ParameterSymbolSpec Parameter);
