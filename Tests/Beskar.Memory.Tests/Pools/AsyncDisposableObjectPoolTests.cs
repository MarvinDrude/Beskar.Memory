using System;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Beskar.Memory.Pools;

namespace Beskar.Memory.Tests.Pools;

public class AsyncDisposableObjectPoolTests
{
   [Fact]
   public async Task AsyncDisposablePoolRecycling()
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
      Assert.True(await pool.ReturnAsync(item));
   }

   [Fact]
   public async Task AsyncDisposablePoolExcessRejectionDisposes()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 0,
         MaxSize = 1
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);

      var item1 = new AsyncDisposableItem();
      var item2 = new AsyncDisposableItem();
      var item3 = new AsyncDisposableItem();

      Assert.True(await pool.ReturnAsync(item1));
      Assert.True(await pool.ReturnAsync(item2));
      Assert.False(await pool.ReturnAsync(item3));

      Assert.False(item1.IsDisposed);
      Assert.False(item2.IsDisposed);
      Assert.True(item3.IsDisposed);
   }

   [Fact]
   public async Task AsyncDisposablePoolDisposalDisposesAll()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 2,
         MaxSize = 4
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);

      var item1 = pool.Get(null);
      var item2 = pool.Get(null);

      await pool.ReturnAsync(item1);
      await pool.ReturnAsync(item2);

      await pool.DisposeAsync();

      Assert.True(item1.IsDisposed);
      Assert.True(item2.IsDisposed);
   }

   [Fact]
   public async Task AsyncDisposablePoolGetAfterDisposeThrows()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 0,
         MaxSize = 2
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);
      await pool.DisposeAsync();

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
   public void AsyncDisposablePoolReturnThrows()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 0,
         MaxSize = 2
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);
      var item = new AsyncDisposableItem();

      try
      {
         pool.Return(item);
         Assert.Fail("Should have thrown InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }
   }

   [Fact]
   public async Task AsyncDisposablePoolReturnAfterDisposeDisposes()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 0,
         MaxSize = 2
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);
      var item = new AsyncDisposableItem();

      await pool.DisposeAsync();

      Assert.False(await pool.ReturnAsync(item));
      Assert.True(item.IsDisposed);
   }

   [Fact]
   public async Task AsyncPoolRentalScope()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 2,
         MaxSize = 4
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);

      await using (var rental = pool.Rent())
      {
         Assert.NotNull(rental.Value);
      }

      // Value should be returned and reusable
      var item = pool.Get(null);
      Assert.NotNull(item);
   }

   [Fact]
   public async Task AsyncHeapPoolRentalScope()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 2,
         MaxSize = 4
      };

      var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);

      AsyncHeapPoolRental<AsyncDisposableItem> rentalOut;
      await using (var rental = pool.RentHeap())
      {
         rentalOut = rental;
         Assert.NotNull(rental.Value);
      }

      // Value should be returned and reusable
      var item = pool.Get(null);
      Assert.NotNull(item);

      // Accessing Value after disposal should throw ObjectDisposedException
      Assert.Throws<ObjectDisposedException>(() => rentalOut.Value);

      // Multiple disposals should be idempotent
      await rentalOut.DisposeAsync();
   }

   [Fact]
   public async Task AsyncDisposablePoolConcurrentReturnAndDisposeNoLeak()
   {
      var options = new ObjectPoolOptions<AsyncDisposableItem>
      {
         FactoryFunc = () => new AsyncDisposableItem(),
         InitialSize = 100,
         MaxSize = 500
      };

      for (var run = 0; run < 10; run++)
      {
         var pool = new AsyncDisposableObjectPool<AsyncDisposableItem>(options);
         var items = new AsyncDisposableItem[100];
         for (var i = 0; i < items.Length; i++)
         {
            items[i] = pool.Get(null);
         }

         var barrier = new Barrier(2);

         var returnTask = Task.Run(async () =>
         {
            barrier.SignalAndWait();
            foreach (var item in items)
            {
               await pool.ReturnAsync(item);
            }
         });

         var disposeTask = Task.Run(async () =>
         {
            barrier.SignalAndWait();
            await pool.DisposeAsync();
         });

         await Task.WhenAll(returnTask, disposeTask);

         // Assert all items must be disposed since pool is disposed
         foreach (var item in items)
         {
            Assert.True(item.IsDisposed);
         }
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
