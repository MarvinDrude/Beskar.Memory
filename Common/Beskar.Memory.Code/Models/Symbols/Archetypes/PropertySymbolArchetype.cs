using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

[DebuggerDisplay("Property: {Symbol.FullName, nq}")]
public readonly record struct PropertySymbolArchetype(
   SymbolSpec Symbol,
   PropertySymbolSpec Property);

