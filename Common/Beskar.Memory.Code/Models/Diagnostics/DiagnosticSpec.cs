using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Models.Diagnostics;

public readonly record struct DiagnosticSpec(
   string DiagnosticId,
   SequenceArray<string> Arguments);

