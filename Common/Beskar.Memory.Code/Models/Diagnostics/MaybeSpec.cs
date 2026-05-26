using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Models.Diagnostics;

/// <summary>
/// Represents a container that optionally holds a value, along with any diagnostic information generated during its production.
/// </summary>
/// <typeparam name="T">The type of the contained value.</typeparam>
/// <param name="HasValue">A value indicating whether a valid value is present.</param>
/// <param name="Value">The contained value, if any.</param>
/// <param name="Diagnostics">The sequence of diagnostics associated with this value container.</param>
public readonly record struct MaybeSpec<T>(
   bool HasValue,
   T Value,
   SequenceArray<DiagnosticSpec> Diagnostics);
