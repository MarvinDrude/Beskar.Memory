using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator.Models;

/// <summary>
/// IsUseConstructor if its a record constructor based type
/// </summary>
public sealed record SerializeSpec(
   NamedTypeSymbolArchetype TypeArchetype,
   bool IsOpenGeneric,
   bool IsUseConstructor,
   SequenceArray<MemberSpec> MemberSpecs,
   SequenceArray<UnionSpec> UnionSpecs,
   SequenceArray<int> ConstructorParameterIndices,
   bool ImplementsSerializationCallback);
