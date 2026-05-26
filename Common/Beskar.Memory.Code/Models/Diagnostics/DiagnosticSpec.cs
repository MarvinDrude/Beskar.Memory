using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Models.Diagnostics;

/// <summary>
/// Represents a diagnostic specification consisting of an identifier and associated arguments.
/// </summary>
/// <param name="DiagnosticId">The unique identifier of the diagnostic.</param>
/// <param name="Arguments">The arguments associated with the diagnostic.</param>
public readonly record struct DiagnosticSpec(
   string DiagnosticId,
   SequenceArray<string> Arguments);
