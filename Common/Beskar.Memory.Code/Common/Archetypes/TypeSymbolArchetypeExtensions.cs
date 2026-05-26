using Beskar.Memory.Code.Common.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;

namespace Beskar.Memory.Code.Common.Archetypes;

/// <summary>
/// Provides extension members for <see cref="TypeSymbolArchetype"/> to assist with metadata query and analysis.
/// </summary>
public static class TypeSymbolArchetypeExtensions
{
   extension(ref TypeSymbolArchetype archetype)
   {
      /// <summary>
      /// Gets a value indicating whether the type archetype represents a <see cref="Guid"/>.
      /// </summary>
      public bool IsGuid => archetype.Symbol.IsGuid;
   }
}
