using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Diagnostics;

public sealed class DiagnosticBuilder<T>(
   int diagnosticInitialCapacity = 16)
   : IDisposable
   where T : new()
{
   private readonly ArrayBuilder<DiagnosticSpec> _diagnostics = new(diagnosticInitialCapacity);
   private bool _isDisposed;
   
   public static DiagnosticBuilder<T> Create(int diagnosticInitialCapacity = 16) 
      => new(diagnosticInitialCapacity);

   public static MaybeSpec<T> CreateSingle(string diagnosticId, params ReadOnlySpan<string> arguments)
   {
      using var builder = Create(1);
      
      return builder
         .Add(diagnosticId, arguments)
         .Build();
   }

   public static MaybeSpec<T> CreateEmpty()
   {
      using var builder = Create(0);
      return builder.Build();
   }

   public DiagnosticBuilder<T> Add(string diagnosticId, params ReadOnlySpan<string> arguments)
   {
      _diagnostics.Add(new DiagnosticSpec(diagnosticId, [.. arguments]));
      return this;
   }

   public MaybeSpec<T> Build()
   {
      if (_isDisposed) 
         throw new InvalidOperationException("Cannot build a disposed DiagnosticBuilder.");
      
      SequenceArray<DiagnosticSpec> diagnostics = [.. _diagnostics.WrittenSpan];
      return new MaybeSpec<T>(false, new T(), diagnostics);
   }
   
   public MaybeSpec<T> Build(T value)
   {
      if (_isDisposed) 
         throw new InvalidOperationException("Cannot build a disposed DiagnosticBuilder.");
      
      SequenceArray<DiagnosticSpec> diagnostics = [.. _diagnostics.WrittenSpan];
      return new MaybeSpec<T>(true, value, diagnostics);
   }

   public void Dispose()
   {
      if (_isDisposed) return;
      
      _diagnostics.Dispose();
      _isDisposed = true;
   }
}

