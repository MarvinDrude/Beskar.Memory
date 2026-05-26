using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Models.Diagnostics;

public readonly record struct MaybeSpec<T>(
   bool HasValue,
   T Value,
   SequenceArray<DiagnosticSpec> Diagnostics);

