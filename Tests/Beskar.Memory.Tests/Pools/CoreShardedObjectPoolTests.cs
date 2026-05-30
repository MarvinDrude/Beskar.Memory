using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Pools;

namespace Beskar.Memory.Tests.Pools;

public class CoreShardedObjectPoolTests
{
   [Fact]
   public void CoreShardedPoolBasicRentReturn()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 4,
         MaxSize = 16
      };

      // Test with custom shard count (Environment.ProcessorCount)
      var pool = new CoreShardedObjectPool<PooledItem>(options, Environment.ProcessorCount);

      var item1 = pool.Get(null);
      var item2 = pool.Get(null);

      Assert.NotNull(item1);
      Assert.NotNull(item2);
      Assert.NotSame(item1, item2);

      Assert.True(pool.Return(item1));
      Assert.True(pool.Return(item2));
   }

   [Fact]
   public void CoreShardedPoolDefaultConstructor()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 4,
         MaxSize = 16
      };

      // Test with default shard count (16)
      var pool = new CoreShardedObjectPool<PooledItem>(options);

      var item = pool.Get(null);
      Assert.NotNull(item);
      Assert.True(pool.Return(item));
   }

   [Fact]
   public void DisposableCoreShardedPoolDisposal()
   {
      var options = new ObjectPoolOptions<DisposableItem>
      {
         FactoryFunc = () => new DisposableItem(),
         InitialSize = 4,
         MaxSize = 16
      };

      var pool = new DisposableCoreShardedObjectPool<DisposableItem>(options, Environment.ProcessorCount);

      var item = pool.Get(null);
      Assert.False(item.IsDisposed);

      pool.Return(item);
      pool.Dispose();

      Assert.True(item.IsDisposed);

      Assert.Throws<InvalidOperationException>(() => pool.Get(null));

      var itemAfter = new DisposableItem();
      Assert.False(pool.Return(itemAfter));
      Assert.True(itemAfter.IsDisposed);
   }

   [Fact]
   public async Task AsyncDisposableCoreShardedPoolDisposal()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 4,
         MaxSize = 16
      };

      var pool = new AsyncDisposableCoreShardedObjectPool<AsyncDisposableItem>(options, Environment.ProcessorCount);

      var item = pool.Get(null);
      Assert.False(item.IsDisposed);

      await pool.ReturnAsync(item);
      await pool.DisposeAsync();

      Assert.True(item.IsDisposed);

      Assert.Throws<InvalidOperationException>(() => pool.Get(null));

      var itemAfter = new AsyncDisposableItem();
      Assert.False(await pool.ReturnAsync(itemAfter));
      Assert.True(itemAfter.IsDisposed);
   }

   [Fact]
   public async Task CoreShardedPoolConcurrencyStressTest()
   {
      var options = new ObjectPoolOptions<DisposableItem>
      {
         FactoryFunc = () => new DisposableItem(),
         InitialSize = 100,
         MaxSize = 1000
      };

      var pool = new DisposableCoreShardedObjectPool<DisposableItem>(options, Environment.ProcessorCount);
      var items = new DisposableItem[200];
      for (var i = 0; i < items.Length; i++)
      {
         items[i] = pool.Get(null);
      }

      var startTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

      var returnTask = Task.Run(() =>
      {
         startTcs.Task.Wait();
         Parallel.For(0, items.Length, i =>
         {
            pool.Return(items[i]);
         });
      });

      var disposeTask = Task.Run(() =>
      {
         startTcs.Task.Wait();
         pool.Dispose();
      });

      // Release concurrently
      startTcs.SetResult();
      await Task.WhenAll(returnTask, disposeTask);

      // Verify that absolutely all items were disposed (no leaks during concurrent teardown)
      foreach (var item in items)
      {
         Assert.True(item.IsDisposed);
      }
   }

   private sealed class PooledItem
   {
      public bool Reusable { get; init; } = true;
   }

   private sealed class DisposableItem : IDisposable
   {
      private volatile bool _isDisposed;
      public bool IsDisposed => _isDisposed;

      public void Dispose()
      {
         _isDisposed = true;
      }
   }

   private sealed class AsyncDisposableItem : IAsyncDisposable
   {
      private volatile bool _isDisposed;
      public bool IsDisposed => _isDisposed;

      public ValueTask DisposeAsync()
      {
         _isDisposed = true;
         return default;
      }
   }
}
