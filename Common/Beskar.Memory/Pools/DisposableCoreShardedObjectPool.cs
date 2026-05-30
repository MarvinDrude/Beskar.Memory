using System;

namespace Beskar.Memory.Pools;

/// <summary>
/// A CPU-core sharded object pool that manages disposable objects, disposing of excess elements upon rejection or pool disposal.
/// </summary>
/// <typeparam name="T">The type of pooled disposable objects.</typeparam>
public sealed class DisposableCoreShardedObjectPool<T> : CoreShardedObjectPool<T>, IDisposable
   where T : class, IDisposable
{
   private volatile bool _isDisposed;

   /// <summary>
   /// Initializes a new instance of the <see cref="DisposableCoreShardedObjectPool{T}"/> class.
   /// </summary>
   /// <param name="options">The options to configure the pool and its shards.</param>
   /// <param name="shardCount">The number of shards to split the pool into. Rounded up to the next power of two.</param>
   public DisposableCoreShardedObjectPool(ObjectPoolOptions<T> options, int shardCount = 16)
      : base(options, static opts => new DisposableObjectPool<T>(opts), shardCount)
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
      ArgumentNullException.ThrowIfNull(item);

      if (_isDisposed)
      {
         item.Dispose();
         return false;
      }

      return base.Return(item);
   }

   /// <summary>
   /// Disposes of the pool, disposing of all idle elements stored inside all core shards.
   /// </summary>
   public void Dispose()
   {
      if (_isDisposed)
      {
         return;
      }
      _isDisposed = true;

      foreach (var shard in _shards)
      {
         ((DisposableObjectPool<T>)shard).Dispose();
      }
   }
}
