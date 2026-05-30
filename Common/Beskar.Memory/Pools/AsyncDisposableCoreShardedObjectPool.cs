using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Beskar.Memory.Pools;

/// <summary>
/// A CPU-core sharded object pool that manages async-disposable objects, disposing of excess elements upon rejection or pool disposal.
/// </summary>
/// <typeparam name="T">The type of pooled async-disposable objects.</typeparam>
public sealed class AsyncDisposableCoreShardedObjectPool<T>
   : CoreShardedObjectPool<T>, IAsyncDisposable
   where T : class, IAsyncDisposable
{
   private volatile bool _isDisposed;

   /// <summary>
   /// Initializes a new instance of the <see cref="AsyncDisposableCoreShardedObjectPool{T}"/> class.
   /// </summary>
   /// <param name="options">The options to configure the pool and its shards.</param>
   /// <param name="shardCount">The number of shards to split the pool into. Rounded up to the next power of two.</param>
   public AsyncDisposableCoreShardedObjectPool(ObjectPoolOptions<T> options, int shardCount = 16)
      : base(options, static opts => new AsyncDisposableObjectPool<T>(opts), shardCount)
   {
   }

   /// <inheritdoc />
   public override T Get(Func<T>? factoryFunc)
   {
      if (_isDisposed)
      {
         throw new InvalidOperationException("Object has been disposed");
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

      var shardIndex = Thread.GetCurrentProcessorId() & (_shards.Length - 1);
      return ((AsyncDisposableObjectPool<T>)_shards[shardIndex]).ReturnAsync(item);

      static async ValueTask<bool> Awaited(T inner)
      {
         await inner.DisposeAsync();
         return false;
      }
   }

   /// <summary>
   /// Disposes of the pool asynchronously, disposing of all idle elements stored inside all core shards.
   /// </summary>
   public async ValueTask DisposeAsync()
   {
      if (_isDisposed)
      {
         return;
      }
      _isDisposed = true;

      foreach (var shard in _shards)
      {
         await ((AsyncDisposableObjectPool<T>)shard)
            .DisposeAsync().ConfigureAwait(false);
      }
   }
}
