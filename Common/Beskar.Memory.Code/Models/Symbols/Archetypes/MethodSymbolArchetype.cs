using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

/// <summary>
/// Represents a method symbol archetype, pairing general symbol details with method-specific specifications.
/// </summary>
/// <param name="Symbol">The general symbol specifications.</param>
/// <param name="Method">The method-specific specifications.</param>
[DebuggerDisplay("Method: {Symbol.FullName, nq}")]
public readonly record struct MethodSymbolArchetype(
   SymbolSpec Symbol,
   MethodSymbolSpec Method);
