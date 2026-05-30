using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Beskar.Memory.Pools;

/// <summary>
/// A CPU-core partitioned sharded object pool designed to eliminate multi-threaded cache-line bouncing and lock contention.
/// Routes threads to core-specific shard pools using fast, division-free bitwise masking over the current processor ID.
/// </summary>
/// <typeparam name="T">The type of pooled objects, which must be reference types.</typeparam>
public class CoreShardedObjectPool<T> : ObjectPool<T>
   where T : class
{
   /// <summary>
   /// Represents the array of partitioned object pool shards.
   /// </summary>
   protected readonly ObjectPool<T>[] _shards;
   private readonly int _shardMask;

   /// <summary>
   /// Initializes a new instance of the <see cref="CoreShardedObjectPool{T}"/> class.
   /// </summary>
   /// <param name="options">The options to configure the pool and its shards.</param>
   /// <param name="shardCount">The number of shards to split the pool into. Rounded up to the next power of two.</param>
   public CoreShardedObjectPool(ObjectPoolOptions<T> options, int shardCount = 16)
      : this(options, static opts => new ObjectPool<T>(opts), shardCount)
   {
   }

   /// <summary>
   /// Internal constructor allowing subclasses to instantiate specialized shard pools (e.g. disposable pools).
   /// </summary>
   protected CoreShardedObjectPool(ObjectPoolOptions<T> options, Func<ObjectPoolOptions<T>, ObjectPool<T>> shardFactory, int shardCount)
      : base(new ObjectPoolOptions<T> { MaxSize = options.MaxSize, FactoryFunc = options.FactoryFunc, ReturnFunc = options.ReturnFunc, InitialSize = 0 })
   {
      ArgumentNullException.ThrowIfNull(options);
      ArgumentNullException.ThrowIfNull(shardFactory);

      if (shardCount <= 0)
      {
         throw new ArgumentOutOfRangeException(nameof(shardCount), "Shard count must be greater than zero.");
      }

      var nextPowerOfTwo = GetNextPowerOfTwo(shardCount);
      _shardMask = nextPowerOfTwo - 1;

      _shards = new ObjectPool<T>[nextPowerOfTwo];

      // Distribute sizes across the shards
      var shardMaxSize = Math.Max(1, options.MaxSize / nextPowerOfTwo);
      var shardInitialSize = options.InitialSize / nextPowerOfTwo;
      var remainderInitialSize = options.InitialSize % nextPowerOfTwo;

      for (var i = 0; i < nextPowerOfTwo; i++)
      {
         var initialSizeForShard = shardInitialSize + (i < remainderInitialSize ? 1 : 0);
         var shardOpts = new ObjectPoolOptions<T>
         {
            FactoryFunc = options.FactoryFunc,
            ReturnFunc = options.ReturnFunc,
            MaxSize = shardMaxSize,
            InitialSize = initialSizeForShard
         };

         _shards[i] = shardFactory(shardOpts);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private int GetShardIndex()
   {
      return Thread.GetCurrentProcessorId() & _shardMask;
   }

   /// <inheritdoc />
   public override T Get(Func<T>? factoryFunc)
   {
      return _shards[GetShardIndex()].Get(factoryFunc);
   }

   /// <inheritdoc />
   public override bool Return(T item)
   {
      return _shards[GetShardIndex()].Return(item);
   }

   private static int GetNextPowerOfTwo(int value)
   {
      var power = 1;
      while (power < value)
      {
         power <<= 1;
      }

      return power;
   }
}
