using System;
using System.Threading;


namespace Beskar.Memory.Pools;

/// <summary>
/// A specialized <see cref="ObjectPool{T}"/> that manages disposable objects, disposing of excess elements upon rejection or pool disposal.
/// </summary>
/// <typeparam name="T">The type of pooled disposable objects.</typeparam>
public sealed class DisposableObjectPool<T>(ObjectPoolOptions<T> options)
   : ObjectPool<T>(options), IDisposable
   where T : class, IDisposable
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
      if (_isDisposed)
      {
         DisposeEntry(item);
         return false;
      }

      if (base.Return(item)) 
      {
         if (_isDisposed)
         {
            DrainPool();
            return false;
         }
         return true;
      }
      
      DisposeEntry(item);
      return false;
   }

   private void DrainPool()
   {
      var head = Interlocked.Exchange(ref _head, null);
      DisposeEntry(head);

      while (_queue.TryDequeue(out var item))
      {
         DisposeEntry(item);
      }
   }

   
   /// <inheritdoc />
   public void Dispose()
   {
      if (_isDisposed)
      {
         return;
      }
      
      _isDisposed = true;

      var head = Interlocked.Exchange(ref _head, null);
      DisposeEntry(head);

      while (_queue.TryDequeue(out var item))
      {
         DisposeEntry(item);
      }
   }
   
   private static void DisposeEntry(T? item)
   {
      item?.Dispose();
   }
   
   private static void ThrowObjectDisposedException()
   {
      throw new InvalidOperationException("Object has been disposed");
   }
}
