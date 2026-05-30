using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;


namespace Beskar.Memory.Pools;

/// <summary>
/// A specialized <see cref="ObjectPool{T}"/> that manages async-disposable objects, disposing of excess elements upon rejection or pool disposal.
/// </summary>
/// <typeparam name="T">The type of pooled async-disposable objects.</typeparam>
public sealed class AsyncDisposableObjectPool<T>(ObjectPoolOptions<T> options)
   : ObjectPool<T>(options), IAsyncDisposable
   where T : class, IAsyncDisposable
{
   private volatile bool _isDisposed;

   /// <summary>
   /// Rents an object from the pool wrapped in a safe, zero-allocation <see cref="AsyncPoolRental{T}"/> scope.
   /// </summary>
   /// <param name="factoryFunc">An optional custom factory delegate to invoke if the pool is empty.</param>
   /// <returns>A safe rental scope wrapping the rented object.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public new AsyncPoolRental<T> Rent(Func<T>? factoryFunc = null)
   {
      return new AsyncPoolRental<T>(this, Get(factoryFunc));
   }


   /// <inheritdoc />
   public override T Get(Func<T>? factoryFunc)
   {
      if (_isDisposed)
      {
         ThrowObjectDisposedException();
      }
      
      return base.Get(factoryFunc);
   }

   /// <inheritdoc />
   public override bool Return(T item)
   {
      throw new InvalidOperationException("Call ReturnAsync instead of Return");
   }

   /// <summary>
   /// Returns an async-disposable object back to the pool, asynchronously disposing of it if the pool is full or disposed.
   /// </summary>
   public ValueTask<bool> ReturnAsync(T item)
   {
      ArgumentNullException.ThrowIfNull(item);

      if (_isDisposed)
      {
         return Awaited(item);
      }

      if (base.Return(item))
      {
         if (_isDisposed)
         {
            return AwaitDrainAndReturnFalse(item);
         }
         return new ValueTask<bool>(true);
      }

      return Awaited(item);

      static async ValueTask<bool> Awaited(T inner)
      {
         await inner.DisposeAsync();
         return false;
      }

      async ValueTask<bool> AwaitDrainAndReturnFalse(T inner)
      {
         await DrainPoolAsync().ConfigureAwait(false);
         return false;
      }
   }

   private async ValueTask DrainPoolAsync()
   {
      var head = Interlocked.Exchange(ref _head, null);
      await DisposeEntryAsync(head).ConfigureAwait(false);

      while (_queue.TryDequeue(out var item))
      {
         await DisposeEntryAsync(item).ConfigureAwait(false);
      }
   }


   /// <inheritdoc />
   public async ValueTask DisposeAsync()
   {
      if (_isDisposed)
      {
         return;
      }
      
      _isDisposed = true;

      var head = Interlocked.Exchange(ref _head, null);
      await DisposeEntryAsync(head).ConfigureAwait(false);
      
      while (_queue.TryDequeue(out var item))
      {
         await DisposeEntryAsync(item).ConfigureAwait(false);
      }
   }

   private static async ValueTask DisposeEntryAsync(T? item)
   {
      if (item is not null)
      {
         await item.DisposeAsync();
      }
   }

   private static void ThrowObjectDisposedException()
   {
      throw new InvalidOperationException("Object has been disposed");
   }
}
