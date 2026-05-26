using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

[DebuggerDisplay("NamedType: {Symbol.FullName, nq}")]
public readonly record struct NamedTypeSymbolArchetype(
   SymbolSpec Symbol,
   TypeSymbolSpec Type,
   NamedTypeSymbolSpec NamedType);

