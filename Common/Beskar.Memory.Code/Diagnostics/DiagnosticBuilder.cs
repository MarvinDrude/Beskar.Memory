using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Diagnostics;

/// <summary>
/// Provides a builder for constructing diagnostic information and results of type <see cref="MaybeSpec{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the underlying specification or value being validated.</typeparam>
/// <param name="diagnosticInitialCapacity">The initial capacity of the underlying diagnostic collection builder.</param>
public sealed class DiagnosticBuilder<T>(
   int diagnosticInitialCapacity = 16)
   : IDisposable
{
   private readonly ArrayBuilder<DiagnosticSpec> _diagnostics = new(diagnosticInitialCapacity);
   private bool _isDisposed;

   /// <summary>
   /// Creates a new instance of the <see cref="DiagnosticBuilder{T}"/> class with the specified initial capacity.
   /// </summary>
   /// <param name="diagnosticInitialCapacity">The initial capacity of the diagnostic list.</param>
   /// <returns>A new <see cref="DiagnosticBuilder{T}"/> instance.</returns>
   public static DiagnosticBuilder<T> Create(int diagnosticInitialCapacity = 16)
      => new(diagnosticInitialCapacity);

   /// <summary>
   /// Creates a single failed diagnostic specification containing one error.
   /// </summary>
   /// <param name="diagnosticId">The unique identifier of the diagnostic.</param>
   /// <param name="arguments">The format arguments for the diagnostic message.</param>
   /// <returns>A <see cref="MaybeSpec{T}"/> containing the single diagnostic failure.</returns>
   public static MaybeSpec<T> CreateSingle(string diagnosticId, params ReadOnlySpan<string> arguments)
   {
      using var builder = Create(1);

      return builder
         .Add(diagnosticId, arguments)
         .Build();
   }

   /// <summary>
   /// Creates an empty failed diagnostic specification with no errors.
   /// </summary>
   /// <returns>A <see cref="MaybeSpec{T}"/> with no value and no diagnostics.</returns>
   public static MaybeSpec<T> CreateEmpty()
   {
      using var builder = Create(0);
      return builder.Build();
   }

   /// <summary>
   /// Adds a diagnostic error specification to the builder.
   /// </summary>
   /// <param name="diagnosticId">The unique identifier of the diagnostic.</param>
   /// <param name="arguments">The format arguments for the diagnostic message.</param>
   /// <returns>The current <see cref="DiagnosticBuilder{T}"/> instance for chaining.</returns>
   public DiagnosticBuilder<T> Add(string diagnosticId, params ReadOnlySpan<string> arguments)
   {
      _diagnostics.Add(new DiagnosticSpec(diagnosticId, [.. arguments]));
      return this;
   }

   /// <summary>
   /// Builds a failed specification containing the gathered diagnostics.
   /// </summary>
   /// <returns>A <see cref="MaybeSpec{T}"/> containing the gathered diagnostics and no value.</returns>
   /// <exception cref="InvalidOperationException">Thrown if the builder has already been disposed.</exception>
   public MaybeSpec<T> Build()
   {
      if (_isDisposed)
         throw new InvalidOperationException("Cannot build a disposed DiagnosticBuilder.");

      SequenceArray<DiagnosticSpec> diagnostics = [.. _diagnostics.WrittenSpan];
      return new MaybeSpec<T>(false, default!, diagnostics);
   }

   /// <summary>
   /// Builds a successful or failed specification containing the gathered diagnostics and the specified value.
   /// </summary>
   /// <param name="value">The successfully built value.</param>
   /// <returns>A <see cref="MaybeSpec{T}"/> containing the value and the gathered diagnostics.</returns>
   /// <exception cref="InvalidOperationException">Thrown if the builder has already been disposed.</exception>
   public MaybeSpec<T> Build(T value)
   {
      if (_isDisposed)
         throw new InvalidOperationException("Cannot build a disposed DiagnosticBuilder.");

      SequenceArray<DiagnosticSpec> diagnostics = [.. _diagnostics.WrittenSpan];
      return new MaybeSpec<T>(true, value, diagnostics);
   }

   /// <summary>
   /// Disposes of the underlying resources used by the <see cref="DiagnosticBuilder{T}"/>.
   /// </summary>
   public void Dispose()
   {
      if (_isDisposed) return;

      _diagnostics.Dispose();
      _isDisposed = true;
   }
}
