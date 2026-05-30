using Beskar.Memory.Code.Models.Symbols.Archetypes;
using System.Collections.Immutable;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.EnumGenerator.Generator.Models;

public readonly record struct EnumFieldDisplayNameSpec(
   string FieldName,
   string? ConstantString,
   string? StaticResourceCall);

public readonly record struct FastEnumSpec(
   NamedTypeSymbolArchetype NamedType,
   SequenceArray<EnumFieldDisplayNameSpec> DisplayNames);
