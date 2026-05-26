using System.Diagnostics;

namespace Beskar.Memory.Code.Models.Symbols.Archetypes;

[DebuggerDisplay("Method: {Symbol.FullName, nq}")]
public readonly record struct MethodSymbolArchetype(
   SymbolSpec Symbol,
   MethodSymbolSpec Method);

