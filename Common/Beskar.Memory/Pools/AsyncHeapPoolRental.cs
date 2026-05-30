using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Beskar.Memory.Pools;

/// <summary>
/// A heap-allocated (class-based) rental wrapper for an async-disposable pooled object.
/// Suitable for long-lived rentals, storing in heap-allocated objects, or async scenarios.
/// Automatically returns the object to the pool asynchronously upon disposal.
/// </summary>
/// <typeparam name="T">The type of pooled objects.</typeparam>
public sealed class AsyncHeapPoolRental<T> : IAsyncDisposable
   where T : class, IAsyncDisposable
{
   private AsyncDisposableObjectPool<T>? _pool;
   private T? _value;

   /// <summary>
   /// Gets the rented object. Throws an <see cref="ObjectDisposedException"/> if the rental has been disposed.
   /// </summary>
   public T Value
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get
      {
         var val = _value;
         if (val is null)
         {
            ThrowObjectDisposedException();
         }
         return val;
      }
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="AsyncHeapPoolRental{T}"/> class.
   /// </summary>
   /// <param name="pool">The pool to return the object to.</param>
   /// <param name="value">The rented object.</param>
   public AsyncHeapPoolRental(AsyncDisposableObjectPool<T> pool, T value)
   {
      ArgumentNullException.ThrowIfNull(pool);
      ArgumentNullException.ThrowIfNull(value);

      _pool = pool;
      _value = value;
   }

   /// <summary>
   /// Returns the rented object to the pool asynchronously and clears the rental reference.
   /// </summary>
   public ValueTask DisposeAsync()
   {
      var pool = _pool;
      var val = _value;

      if (pool is not null && val is not null)
      {
         _pool = null;
         _value = null;

         var vt = pool.ReturnAsync(val);
         return vt.IsCompletedSuccessfully
            ? ValueTask.CompletedTask : Await(vt);
      }

      return ValueTask.CompletedTask;

      static async ValueTask Await(ValueTask<bool> vt)
      {
         await vt;
      }
   }

   [DoesNotReturn]
   private static void ThrowObjectDisposedException()
   {
      throw new ObjectDisposedException(nameof(AsyncHeapPoolRental<T>), "The pool rental has already been disposed.");
   }
}
