using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Writers;

/// <summary>
/// A lightweight, stack-safe, zero-allocation resource handle that wraps an <see cref="ArrayBuilder{T}"/>.
/// Exposes a safe view of the built <see cref="Span{T}"/> and automatically returns the builder's rented memory to the pool when disposed.
/// </summary>
/// <typeparam name="T">The type of items stored in the buffer.</typeparam>
public readonly struct ArrayBuilderResult<T> : IDisposable
{
   private readonly ArrayBuilder<T>? _builder;

   /// <summary>
   /// Gets a value indicating whether this result carries an active builder value.
   /// </summary>
   [MemberNotNullWhen(true, nameof(_builder))]
   public bool HasValue => _builder is not null;

   /// <summary>
   /// Gets a mutable span representing the already written data, or <see cref="Span{T}.Empty"/> if empty.
   /// </summary>
   public Span<T> WrittenSpan
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _builder is null ? Span<T>.Empty : _builder.WrittenSpan;
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="ArrayBuilderResult{T}"/> struct wrapping the specified builder.
   /// </summary>
   /// <param name="builder">The array builder to wrap and take ownership of.</param>
   public ArrayBuilderResult(ArrayBuilder<T>? builder)
   {
      _builder = builder;
   }

   /// <summary>
   /// Gets a default, empty <see cref="ArrayBuilderResult{T}"/>.
   /// </summary>
   public static ArrayBuilderResult<T> Empty => new(null);

   /// <summary>
   /// Disposes the underlying builder, returning any rented buffers to the pool.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Dispose()
   {
      _builder?.Dispose();
   }

   /// <summary>
   /// Implicitly converts an <see cref="ArrayBuilder{T}"/> to an <see cref="ArrayBuilderResult{T}"/> wrapping it.
   /// </summary>
   public static implicit operator ArrayBuilderResult<T>(ArrayBuilder<T>? builder) => new(builder);

   /// <summary>
   /// Implicitly converts the <see cref="ArrayBuilderResult{T}"/> to a mutable <see cref="Span{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Span<T>(ArrayBuilderResult<T> result) => result.WrittenSpan;

   /// <summary>
   /// Implicitly converts the <see cref="ArrayBuilderResult{T}"/> to a <see cref="ReadOnlySpan{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator ReadOnlySpan<T>(ArrayBuilderResult<T> result) => result.WrittenSpan;
}
