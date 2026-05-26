using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Pools;

namespace Beskar.Memory.Tests.Pools;

public class PoolsTests
{
   [Fact]
   public async Task IoQueueScheduleExecution()
   {
      var queue = new IoQueue();
      var tcs = new TaskCompletionSource<int>();
      var result = 0;

      queue.Schedule(state =>
      {
         result = (int)state!;
         tcs.TrySetResult(result);
      }, 42);

      var executed = await tcs.Task;
      Assert.Equal(42, executed);
      Assert.Equal(42, result);
   }

   [Fact]
   public void PinnedBlockMemoryPoolRentAndDispose()
   {
      using var pool = new PinnedBlockMemoryPool();
      
      Assert.Equal(4096, pool.MaxBufferSize);
      
      using var block1 = pool.Rent(2048);
      Assert.NotNull(block1);
      Assert.Equal(4096, block1.Memory.Length);
      
      Assert.Throws<ArgumentOutOfRangeException>(() => pool.Rent(8192));
      
      var block2 = pool.Rent();
      block2.Dispose();

      using var block3 = pool.Rent();
      Assert.Same(block2, block3);
   }

   [Fact]
   public void ObjectPoolBasicOperations()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 2,
         MaxSize = 4
      };

      var pool = new ObjectPool<PooledItem>(options);

      var item1 = pool.Get(null);
      var item2 = pool.Get(null);
      var item3 = pool.Get(null);

      Assert.NotNull(item1);
      Assert.NotNull(item2);
      Assert.NotNull(item3);

      Assert.True(pool.Return(item1));
      Assert.True(pool.Return(item2));
      Assert.True(pool.Return(item3));
      
      var item4 = pool.Get(null);
      Assert.Same(item1, item4);

      var returned = pool.Return(new PooledItem());
      Assert.True(returned);
   }

   [Fact]
   public void DisposableObjectPoolOperations()
   {
      var options = new ObjectPoolOptions<DisposableItem>
      {
         FactoryFunc = () => new DisposableItem(),
         InitialSize = 1,
         MaxSize = 2
      };

      var pool = new DisposableObjectPool<DisposableItem>(options);
      var item = pool.Get(null);
      
      Assert.False(item.IsDisposed);
      
      pool.Return(item);
      pool.Dispose();
      
      Assert.True(item.IsDisposed);

      try
      {
         pool.Get(null);
         Assert.Fail("Should have thrown InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }
   }

   [Fact]
   public async Task AsyncDisposableObjectPoolOperations()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 1,
         MaxSize = 2
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);
      var item = pool.Get(null);

      Assert.False(item.IsDisposed);

      try
      {
         pool.Return(item);
         Assert.Fail("Should have thrown InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }

      var returned = await pool.ReturnAsync(item);
      Assert.True(returned);

      await pool.DisposeAsync();
      Assert.True(item.IsDisposed);

      try
      {
         pool.Get(null);
         Assert.Fail("Should have thrown InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }
   }

   private sealed class PooledItem;

   private sealed class DisposableItem : IDisposable
   {
      public bool IsDisposed { get; private set; }

      public void Dispose()
      {
         IsDisposed = true;
      }
   }

   private sealed class AsyncDisposableItem : IAsyncDisposable
   {
      public bool IsDisposed { get; private set; }

      public ValueTask DisposeAsync()
      {
         IsDisposed = true;
         return default;
      }
   }
}
