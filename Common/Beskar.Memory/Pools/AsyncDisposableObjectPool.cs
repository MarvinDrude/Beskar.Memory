using System;
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

      if (_isDisposed || !base.Return(item))
      {
         return Awaited(item);
      }

      return new ValueTask<bool>(true);

      static async ValueTask<bool> Awaited(T inner)
      {
         await inner.DisposeAsync();
         return false;
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

      await DisposeEntryAsync(_head);
      _head = null;
      
      while (_queue.TryDequeue(out var item))
      {
         await DisposeEntryAsync(item);
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
