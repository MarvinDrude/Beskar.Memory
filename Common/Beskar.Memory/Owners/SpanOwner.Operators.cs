using System.Runtime.CompilerServices;

namespace Beskar.Memory.Owners;

public ref partial struct SpanOwner<T>
{
   /// <summary>
   /// Implicitly converts this <see cref="SpanOwner{T}"/> to a <see cref="Span{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Span<T>(SpanOwner<T> owner) => owner.Span;

   /// <summary>
   /// Implicitly converts this <see cref="SpanOwner{T}"/> to a <see cref="ReadOnlySpan{T}"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator ReadOnlySpan<T>(SpanOwner<T> owner) => owner.Span;
}
