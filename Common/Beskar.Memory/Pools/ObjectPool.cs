using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Beskar.Memory.Pools;

/// <summary>
/// A high-performance, thread-safe object pool designed for zero-allocation object recycling with a lock-free fast-path.
/// </summary>
/// <typeparam name="T">The type of pooled objects, which must be reference types.</typeparam>
public class ObjectPool<T>
   where T : class
{
   private readonly Func<T> _factoryFunc;
   private readonly Func<T, bool> _returnFunc;

   private readonly int _maxSize;
   private readonly int _initialSize;

   private int _currentSize;
   
   /// <summary>
   /// Represents the concurrent fallback queue for this pool.
   /// </summary>
   protected readonly ConcurrentQueue<T> _queue = [];

   /// <summary>
   /// Represents the lock-free head slot for high-performance retrieval.
   /// </summary>
   protected T? _head;

   /// <summary>
   /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
   /// </summary>
   /// <param name="options">The options to configure the pool.</param>
   public ObjectPool(ObjectPoolOptions<T> options)
   {
      ArgumentNullException.ThrowIfNull(options);
      
      _factoryFunc = options.FactoryFunc;
      _returnFunc = options.ReturnFunc;
      
      _maxSize = options.MaxSize;
      _initialSize = options.InitialSize;

      for (var i = 0; i < options.InitialSize; i++)
      {
         _queue.Enqueue(_factoryFunc());
      }

      _queue.TryDequeue(out _head);
      _currentSize = _queue.Count;
   }
   
   /// <summary>
   /// Gets a pooled object. If the pool is empty, executes a new instantiation using the provided or default factory delegate.
   /// </summary>
   /// <param name="factoryFunc">An optional custom factory delegate to invoke if the pool is empty.</param>
   /// <returns>A recycled or newly instantiated object.</returns>
   public virtual T Get(Func<T>? factoryFunc)
   {
      var candidate = _head;
      
      if (candidate == null || Interlocked.CompareExchange(ref _head, null, candidate) != candidate)
      {
         if (!_queue.TryDequeue(out candidate))
         {
            return factoryFunc?.Invoke() ?? _factoryFunc();
         }

         Interlocked.Decrement(ref _currentSize);
      }
      
      return candidate;
   }

   /// <summary>
   /// Returns the object back to the pool for recycling if it satisfies the return predicate and pool capacity is not exceeded.
   /// </summary>
   /// <param name="item">The object to return.</param>
   /// <returns><see langword="true"/> if the object was recycled; otherwise <see langword="false"/>.</returns>
   public virtual bool Return(T item)
   {
      ArgumentNullException.ThrowIfNull(item);

      if (!_returnFunc(item))
      {
         return false;
      }

      if (_head != null || Interlocked.CompareExchange(ref _head, item, null) != null)
      {
         if (Interlocked.Increment(ref _currentSize) <= _maxSize)
         {
            _queue.Enqueue(item);
            return true;
         }
         
         Interlocked.Decrement(ref _currentSize);
         return false;
      }

      return true;
   }
}
