using System;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Pools;

namespace Beskar.Memory.Tests.Pools;

public class ObjectPoolTests
{
   [Fact]
   public void PoolGetAndReturn()
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

      Assert.NotNull(item1);
      Assert.NotNull(item2);

      Assert.True(pool.Return(item1));
      Assert.True(pool.Return(item2));

      var item3 = pool.Get(null);
      Assert.Same(item1, item3);
   }

   [Fact]
   public void PoolFactoryFallback()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 0,
         MaxSize = 2
      };

      var pool = new ObjectPool<PooledItem>(options);
      
      var item = pool.Get(() => new PooledItem { Custom = true });
      Assert.True(item.Custom);
   }

   [Fact]
   public void PoolMaxSizeCapacity()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 0,
         MaxSize = 1
      };

      var pool = new ObjectPool<PooledItem>(options);

      var item1 = new PooledItem();
      var item2 = new PooledItem();
      var item3 = new PooledItem();

      Assert.True(pool.Return(item1));
      Assert.True(pool.Return(item2));
      Assert.False(pool.Return(item3));
   }

   [Fact]
   public void PoolReturnPredicateRejection()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         ReturnFunc = item => item.Reusable,
         InitialSize = 0,
         MaxSize = 2
      };

      var pool = new ObjectPool<PooledItem>(options);

      var item1 = new PooledItem { Reusable = true };
      var item2 = new PooledItem { Reusable = false };

      Assert.True(pool.Return(item1));
      Assert.False(pool.Return(item2));
   }

   [Fact]
   public void PoolConcurrentGetsAndReturns()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 10,
         MaxSize = 50
      };

      var pool = new ObjectPool<PooledItem>(options);

      Parallel.For(0, 500, i =>
      {
         var item = pool.Get(null);
         Assert.NotNull(item);
         pool.Return(item);
      });
   }

   [Fact]
   public void PoolRentalScope()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 2,
         MaxSize = 4
      };

      var pool = new ObjectPool<PooledItem>(options);

      using (var rental = pool.Rent())
      {
         Assert.NotNull(rental.Value);
      }

      // Value should be returned and reusable
      var item = pool.Get(null);
      Assert.NotNull(item);
   }

   [Fact]
   public void HeapPoolRentalScope()
   {
      var options = new ObjectPoolOptions<PooledItem>
      {
         FactoryFunc = () => new PooledItem(),
         InitialSize = 2,
         MaxSize = 4
      };

      var pool = new ObjectPool<PooledItem>(options);

      HeapPoolRental<PooledItem> rentalOut;
      using (var rental = pool.RentHeap())
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
      rentalOut.Dispose();
   }

   private sealed class PooledItem
   {
      public bool Custom { get; init; }
      public bool Reusable { get; init; } = true;
   }
}
