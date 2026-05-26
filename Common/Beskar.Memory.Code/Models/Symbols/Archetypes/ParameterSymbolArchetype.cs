using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

[DebuggerDisplay("Parameter: {Symbol.FullName, nq}")]
public readonly record struct ParameterSymbolArchetype(
   SymbolSpec Symbol,
   ParameterSymbolSpec Parameter);

