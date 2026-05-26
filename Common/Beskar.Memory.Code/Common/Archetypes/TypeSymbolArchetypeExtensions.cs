using Beskar.Memory.Code.Common.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;

namespace Beskar.Memory.Code.Common.Archetypes;

public static class TypeSymbolArchetypeExtensions
{
   extension(ref TypeSymbolArchetype archetype)
   {
      public bool IsGuid => archetype.Symbol.IsGuid;
   }
}

