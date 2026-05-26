using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Owners;

public partial struct MemoryOwner<T>
{
   /// <summary>
   /// Implicitly converts this <see cref="MemoryOwner{T}"/> to a <see cref="Memory{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Memory<T>(MemoryOwner<T> owner) => owner.Memory;

   /// <summary>
   /// Implicitly converts this <see cref="MemoryOwner{T}"/> to a <see cref="ReadOnlyMemory{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator ReadOnlyMemory<T>(MemoryOwner<T> owner) => owner.Memory;

   /// <summary>
   /// Implicitly converts this <see cref="MemoryOwner{T}"/> to a <see cref="Span{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Span<T>(MemoryOwner<T> owner) => owner.Span;

   /// <summary>
   /// Implicitly converts this <see cref="MemoryOwner{T}"/> to a <see cref="ReadOnlySpan{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator ReadOnlySpan<T>(MemoryOwner<T> owner) => owner.Span;
}
