using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.ObserveGenerator.Generator.Models;

public readonly record struct ObserveSpec(
   NamedTypeSymbolArchetype NamedTypeArchetype,
   ObserveActivitySpec? ActivitySpec,
   ObserveMeterSpec? MeterSpec,
   SequenceArray<ObserveInstrumentSpec> InstrumentSpecs);
