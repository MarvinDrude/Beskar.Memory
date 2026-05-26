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

   /// <summary>
   /// Allocates a new pooled buffer and copies the contents of the source span into it.
   /// </summary>
   /// <param name="source">The source data to duplicate.</param>
   /// <param name="minSize">The min size of the new buffer.</param>
   /// <param name="clearArray"><see langword="true"/> to clear the rented buffer; otherwise, <see langword="false"/>.</param>
   /// <param name="pool">The custom pool to use, or null for the shared pool.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static SpanOwner<T> AllocateAndCopy(ReadOnlySpan<T> source, int minSize,
      bool clearArray = false, ArrayPool<T>? pool = null)
   {
      // Set clearArray to false because we immediately overwrite every single byte!
      var owner = new SpanOwner<T>(minSize, clearArray: clearArray, pool);
      source.CopyTo(owner);

      return owner;
   }
}
