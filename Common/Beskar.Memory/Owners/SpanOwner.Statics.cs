using System.Buffers;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Owners;

public ref partial struct SpanOwner<T>
{
   /// <summary>
   /// Gets a default, empty <see cref="SpanOwner{T}"/>.
   /// </summary>
   public static SpanOwner<T> Empty
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => default;
   }

   /// <summary>
   /// Rents a new buffer of the specified size. A clean, fluent alternative to the constructor.
   /// </summary>
   /// <param name="size">The minimum number of elements required in the buffer.</param>
   /// <param name="clearArray"><see langword="true"/> to clear the rented buffer; otherwise, <see langword="false"/>.</param>
   /// <param name="pool">The custom <see cref="ArrayPool{T}"/> to rent from, or <see langword="null"/> to use the shared pool.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static SpanOwner<T> Allocate(int size, bool clearArray = true, ArrayPool<T>? pool = null)
   {
      return new SpanOwner<T>(size, clearArray, pool);
   }
}
