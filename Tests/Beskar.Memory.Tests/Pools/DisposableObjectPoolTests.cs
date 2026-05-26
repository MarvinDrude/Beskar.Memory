using System;
using Xunit;
using Beskar.Memory.Pools;

namespace Beskar.Memory.Tests.Pools;

public class DisposableObjectPoolTests
{
   [Fact]
   public void DisposablePoolRecycling()
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
      Assert.True(pool.Return(item));
   }

   [Fact]
   public void DisposablePoolExcessRejectionDisposes()
   {
      var options = new ObjectPoolOptions<DisposableItem>
      {
         FactoryFunc = () => new DisposableItem(),
         InitialSize = 0,
         MaxSize = 1
      };

      var pool = new DisposableObjectPool<DisposableItem>(options);
      
      var item1 = new DisposableItem();
      var item2 = new DisposableItem();
      var item3 = new DisposableItem();

      Assert.True(pool.Return(item1));
      Assert.True(pool.Return(item2));
      Assert.False(pool.Return(item3));
      
      Assert.False(item1.IsDisposed);
      Assert.False(item2.IsDisposed);
      Assert.True(item3.IsDisposed);
   }

   [Fact]
   public void DisposablePoolDisposalDisposesAll()
   {
      var options = new ObjectPoolOptions<DisposableItem>
      {
         FactoryFunc = () => new DisposableItem(),
         InitialSize = 2,
         MaxSize = 4
      };

      var pool = new DisposableObjectPool<DisposableItem>(options);
      
      var item1 = pool.Get(null);
      var item2 = pool.Get(null);

      pool.Return(item1);
      pool.Return(item2);

      pool.Dispose();

      Assert.True(item1.IsDisposed);
      Assert.True(item2.IsDisposed);
   }

   [Fact]
   public void DisposablePoolGetAfterDisposeThrows()
   {
      var options = new ObjectPoolOptions<DisposableItem>
      {
         FactoryFunc = () => new DisposableItem(),
         InitialSize = 0,
         MaxSize = 2
      };

      var pool = new DisposableObjectPool<DisposableItem>(options);
      pool.Dispose();

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
   public void DisposablePoolReturnAfterDisposeDisposes()
   {
      var options = new ObjectPoolOptions<DisposableItem>
      {
         FactoryFunc = () => new DisposableItem(),
         InitialSize = 0,
         MaxSize = 2
      };

      var pool = new DisposableObjectPool<DisposableItem>(options);
      var item = new DisposableItem();

      pool.Dispose();
      
      Assert.False(pool.Return(item));
      Assert.True(item.IsDisposed);
   }

   private sealed class DisposableItem : IDisposable
   {
      public bool IsDisposed { get; private set; }

      public void Dispose()
      {
         IsDisposed = true;
      }
   }
}
