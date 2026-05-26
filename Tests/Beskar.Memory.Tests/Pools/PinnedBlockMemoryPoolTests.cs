using System;
using System.Buffers;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Pools;

namespace Beskar.Memory.Tests.Pools;

public class PinnedBlockMemoryPoolTests
{
   [Fact]
   public void RentBasic()
   {
      using var pool = new PinnedBlockMemoryPool();
      
      Assert.Equal(4096, pool.MaxBufferSize);
      
      using var block = pool.Rent();
      Assert.NotNull(block);
      Assert.Equal(4096, block.Memory.Length);
   }

   [Fact]
   public void RentVariousSizes()
   {
      using var pool = new PinnedBlockMemoryPool();
      
      using var block1 = pool.Rent(512);
      using var block2 = pool.Rent(1024);
      using var block3 = pool.Rent(4096);

      Assert.Equal(4096, block1.Memory.Length);
      Assert.Equal(4096, block2.Memory.Length);
      Assert.Equal(4096, block3.Memory.Length);
   }

   [Fact]
   public void RentBeyondBlockSizeThrows()
   {
      using var pool = new PinnedBlockMemoryPool();
      
      Assert.Throws<ArgumentOutOfRangeException>(() => pool.Rent(4097));
      Assert.Throws<ArgumentOutOfRangeException>(() => pool.Rent(8192));
   }

   [Fact]
   public void RentAfterDisposeThrows()
   {
      var pool = new PinnedBlockMemoryPool();
      pool.Dispose();

      Assert.Throws<ObjectDisposedException>(() => pool.Rent());
   }

   [Fact]
   public void ReturnedBlocksAreRecycled()
   {
      using var pool = new PinnedBlockMemoryPool();
      
      var block1 = pool.Rent();
      block1.Dispose();

      using var block2 = pool.Rent();
      Assert.Same(block1, block2);
   }

   [Fact]
   public void ConcurrentRentAndReturn()
   {
      using var pool = new PinnedBlockMemoryPool();
      
      Parallel.For(0, 100, i =>
      {
         using var block = pool.Rent();
         Assert.Equal(4096, block.Memory.Length);
      });
   }
}
