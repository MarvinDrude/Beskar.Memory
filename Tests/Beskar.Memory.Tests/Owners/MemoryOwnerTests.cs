using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Owners;

namespace Beskar.Memory.Tests.Owners;

public class MemoryOwnerTests
{
   private class CustomArrayPool<T> : ArrayPool<T>
   {
      public int RentCount { get; set; }
      public int ReturnCount { get; set; }
      public T[]? LastReturnedArray { get; set; }
      public bool LastClearArray { get; set; }

      public override T[] Rent(int minimumLength)
      {
         RentCount++;
         
         return new T[minimumLength];
      }

      public override void Return(T[] array, bool clearArray = false)
      {
         ReturnCount++;
         LastReturnedArray = array;
         LastClearArray = clearArray;
      }
   }

   [Fact]
   public void EmptyReturnsDefaultState()
   {
      var owner = MemoryOwner<int>.Empty;
      
      Assert.Equal(0, owner.Length);
      Assert.Equal(0, owner.Capacity);
      Assert.True(owner.Span.IsEmpty);
      Assert.True(owner.Memory.IsEmpty);
      Assert.Empty(owner.Buffer);
      
      owner.Dispose();
   }

   [Fact]
   public void DefaultConstructorReturnsDefaultState()
   {
      var owner = new MemoryOwner<int>();
      
      Assert.Equal(0, owner.Length);
      Assert.Equal(0, owner.Capacity);
      Assert.True(owner.Span.IsEmpty);
      Assert.True(owner.Memory.IsEmpty);
      Assert.Empty(owner.Buffer);
      
      owner.Dispose();
   }

   [Fact]
   public void ConstructorZeroSizeReturnsDefaultState()
   {
      var owner = new MemoryOwner<int>(0);
      
      Assert.Equal(0, owner.Length);
      Assert.Equal(0, owner.Capacity);
      Assert.True(owner.Span.IsEmpty);
      Assert.True(owner.Memory.IsEmpty);
      Assert.Empty(owner.Buffer);
      
      owner.Dispose();
   }

   [Fact]
   public void ConstructorNegativeSizeThrowsArgumentOutOfRangeException()
   {
      var threw = false;
      try
      {
         var owner = new MemoryOwner<int>(-1);
      }
      catch (ArgumentOutOfRangeException)
      {
         threw = true;
      }
      
      Assert.True(threw);
   }

   [Fact]
   public void ConstructorExternalArrayWrapsCorrectly()
   {
      int[] array = [1, 2, 3, 4, 5];
      var owner = new MemoryOwner<int>(array);
      
      Assert.Equal(5, owner.Length);
      Assert.Equal(5, owner.Capacity);
      Assert.Equal(1, owner[0]);
      Assert.Equal(5, owner[4]);
      Assert.Same(array, owner.Buffer);
      
      owner[0] = 10;
      
      Assert.Equal(10, array[0]);
      
      owner.Dispose();
      
      var threwLength = false;
      try
      {
         var _ = owner.Length;
      }
      catch (ObjectDisposedException)
      {
         threwLength = true;
      }
      
      Assert.True(threwLength);
   }

   [Fact]
   public void ConstructorRentedBufferUsesPool()
   {
      var pool = new CustomArrayPool<int>();
      var owner = new MemoryOwner<int>(10, true, pool);
      
      Assert.Equal(1, pool.RentCount);
      Assert.Equal(10, owner.Length);
      Assert.Equal(10, owner.Span.Length);
      Assert.Equal(10, owner.Memory.Length);
      
      var arraySegment = owner.DangerousGetArray();
      
      Assert.NotNull(arraySegment.Array);
      Assert.Equal(0, arraySegment.Offset);
      Assert.Equal(10, arraySegment.Count);
      
      owner.Dispose();
      
      Assert.Equal(1, pool.ReturnCount);
   }

   [Fact]
   public void IndexerOutOfBoundsThrowsIndexOutOfRangeException()
   {
      var owner = new MemoryOwner<int>(5);
      
      var threwLower = false;
      try
      {
         var _ = owner[-1];
      }
      catch (IndexOutOfRangeException)
      {
         threwLower = true;
      }
      
      Assert.True(threwLower);
      
      var threwUpper = false;
      try
      {
         var _ = owner[5];
      }
      catch (IndexOutOfRangeException)
      {
         threwUpper = true;
      }
      
      Assert.True(threwUpper);
      
      owner.Dispose();
   }

   [Fact]
   public void LengthSetterResizesCorrectly()
   {
      var owner = new MemoryOwner<int>(10);
      
      owner.Length = 5;
      
      Assert.Equal(5, owner.Length);
      Assert.Equal(5, owner.Span.Length);
      Assert.Equal(5, owner.Memory.Length);
      
      owner.Dispose();
   }

   [Fact]
   public void LengthSetterThrowsArgumentOutOfRangeException()
   {
      var owner = new MemoryOwner<int>(10);
      
      var threwLower = false;
      try
      {
         owner.Length = -1;
      }
      catch (ArgumentOutOfRangeException)
      {
         threwLower = true;
      }
      
      Assert.True(threwLower);
      
      var threwUpper = false;
      try
      {
         owner.Length = owner.Capacity + 5;
      }
      catch (ArgumentOutOfRangeException)
      {
         threwUpper = true;
      }
      
      Assert.True(threwUpper);
      
      owner.Dispose();
   }

   [Fact]
   public void TryResizeResizesCorrectly()
   {
      var owner = new MemoryOwner<int>(10);
      
      var resized = owner.TryResize(5);
      
      Assert.True(resized);
      Assert.Equal(5, owner.Length);
      Assert.Equal(5, owner.Span.Length);
      Assert.Equal(5, owner.Memory.Length);
      
      owner.Dispose();
   }

   [Fact]
   public void TryResizeReturnsFalseIfNewSizeExceedsCapacity()
   {
      var owner = new MemoryOwner<int>(10);
      
      var resized = owner.TryResize(owner.Capacity + 5);
      
      Assert.False(resized);
      Assert.Equal(10, owner.Length);
      
      owner.Dispose();
   }

   [Fact]
   public void TryResizeThrowsArgumentOutOfRangeException()
   {
      var owner = new MemoryOwner<int>(10);
      
      var threw = false;
      try
      {
         owner.TryResize(-1);
      }
      catch (ArgumentOutOfRangeException)
      {
         threw = true;
      }
      
      Assert.True(threw);
      
      owner.Dispose();
   }

   [Fact]
   public void FillFillsAllElements()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      
      for (var i = 0; i < owner.Length; i++)
      {
         Assert.Equal(42, owner[i]);
      }
      
      owner.Dispose();
   }

   [Fact]
   public void ClearResetsAllElements()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      owner.Clear();
      
      for (var i = 0; i < owner.Length; i++)
      {
         Assert.Equal(0, owner[i]);
      }
      
      owner.Dispose();
   }

   [Fact]
   public void CopyToSpanCopiesElements()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      
      var dest = new int[5];
      owner.CopyTo(dest.AsSpan());
      
      for (var i = 0; i < dest.Length; i++)
      {
         Assert.Equal(42, dest[i]);
      }
      
      owner.Dispose();
   }

   [Fact]
   public void CopyToMemoryCopiesElements()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      
      var dest = new Memory<int>(new int[5]);
      owner.CopyTo(dest);
      
      var destSpan = dest.Span;
      for (var i = 0; i < destSpan.Length; i++)
      {
         Assert.Equal(42, destSpan[i]);
      }
      
      owner.Dispose();
   }

   [Fact]
   public void TryCopyToSpanSucceedsWhenDestinationLargeEnough()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      
      var dest = new int[10];
      
      Assert.True(owner.TryCopyTo(dest.AsSpan()));
      
      for (var i = 0; i < 5; i++)
      {
         Assert.Equal(42, dest[i]);
      }
      
      owner.Dispose();
   }

   [Fact]
   public void TryCopyToSpanFailsWhenDestinationTooSmall()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      
      var dest = new int[3];
      
      Assert.False(owner.TryCopyTo(dest.AsSpan()));
      
      owner.Dispose();
   }

   [Fact]
   public void TryCopyToMemorySucceedsWhenDestinationLargeEnough()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      
      var dest = new Memory<int>(new int[10]);
      
      Assert.True(owner.TryCopyTo(dest));
      
      var destSpan = dest.Span;
      for (var i = 0; i < 5; i++)
      {
         Assert.Equal(42, destSpan[i]);
      }
      
      owner.Dispose();
   }

   [Fact]
   public void TryCopyToMemoryFailsWhenDestinationTooSmall()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Fill(42);
      
      var dest = new Memory<int>(new int[3]);
      
      Assert.False(owner.TryCopyTo(dest));
      
      owner.Dispose();
   }

   [Fact]
   public void SliceSlicesCorrectly()
   {
      var owner = new MemoryOwner<int>(10);
      
      for (var i = 0; i < 10; i++)
      {
         owner[i] = i;
      }
      
      var slice = owner.Slice(3, 4);
      
      Assert.Equal(4, slice.Length);
      Assert.Equal(3, slice[0]);
      Assert.Equal(6, slice[3]);
      
      owner.Dispose();
   }

   [Fact]
   public void GetEnumeratorCanIterate()
   {
      var owner = new MemoryOwner<int>(3);
      
      owner[0] = 10;
      owner[1] = 20;
      owner[2] = 30;
      
      var sum = 0;
      foreach (var val in owner)
      {
         sum += val;
      }
      
      Assert.Equal(60, sum);
      
      owner.Dispose();
   }

   [Fact]
   public void AllocateMatchesParameters()
   {
      var pool = new CustomArrayPool<int>();
      var owner = MemoryOwner<int>.Allocate(15, true, pool);
      
      Assert.Equal(1, pool.RentCount);
      Assert.Equal(15, owner.Length);
      
      owner.Dispose();
      
      Assert.Equal(1, pool.ReturnCount);
   }

   [Fact]
   public void AllocateAndCopyAllocatesAndCopies()
   {
      int[] source = [1, 2, 3];
      var pool = new CustomArrayPool<int>();
      var owner = MemoryOwner<int>.AllocateAndCopy(source, 5, false, pool);
      
      Assert.Equal(5, owner.Length);
      Assert.Equal(1, owner[0]);
      Assert.Equal(2, owner[1]);
      Assert.Equal(3, owner[2]);
      
      owner.Dispose();
   }

   [Fact]
   public void DisposeReturnsValueTypeWithClearArrayFalse()
   {
      var pool = new CustomArrayPool<int>();
      var owner = new MemoryOwner<int>(5, false, pool);
      
      owner.Dispose();
      
      Assert.Equal(1, pool.ReturnCount);
      Assert.False(pool.LastClearArray);
   }

   [Fact]
   public void DisposeReturnsReferenceTypeWithClearArrayTrue()
   {
      var pool = new CustomArrayPool<string>();
      var owner = new MemoryOwner<string>(5, false, pool);
      
      owner.Dispose();
      
      Assert.Equal(1, pool.ReturnCount);
      Assert.True(pool.LastClearArray);
   }

   [Fact]
   public void DoubleDisposeOnlyReturnsOnce()
   {
      var pool = new CustomArrayPool<int>();
      var owner = new MemoryOwner<int>(5, false, pool);
      
      owner.Dispose();
      owner.Dispose();
      
      Assert.Equal(1, pool.ReturnCount);
   }

   [Fact]
   public void AccessDisposedThrowsObjectDisposedException()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Dispose();
      
      var threwLength = false;
      try
      {
         var _ = owner.Length;
      }
      catch (ObjectDisposedException)
      {
         threwLength = true;
      }
      
      Assert.True(threwLength);
      
      var threwSpan = false;
      try
      {
         var _ = owner.Span;
      }
      catch (ObjectDisposedException)
      {
         threwSpan = true;
      }
      
      Assert.True(threwSpan);
      
      var threwMemory = false;
      try
      {
         var _ = owner.Memory;
      }
      catch (ObjectDisposedException)
      {
         threwMemory = true;
      }
      
      Assert.True(threwMemory);
      
      var threwIndexer = false;
      try
      {
         var _ = owner[0];
      }
      catch (ObjectDisposedException)
      {
         threwIndexer = true;
      }
      
      Assert.True(threwIndexer);
      
      var threwArray = false;
      try
      {
         owner.DangerousGetArray();
      }
      catch (ObjectDisposedException)
      {
         threwArray = true;
      }
      
      Assert.True(threwArray);
      
      var threwBuffer = false;
      try
      {
         var _ = owner.Buffer;
      }
      catch (ObjectDisposedException)
      {
         threwBuffer = true;
      }
      
      Assert.True(threwBuffer);
      
      var threwCapacity = false;
      try
      {
         var _ = owner.Capacity;
      }
      catch (ObjectDisposedException)
      {
         threwCapacity = true;
      }
      
      Assert.True(threwCapacity);
      
      var threwResize = false;
      try
      {
         owner.TryResize(2);
      }
      catch (ObjectDisposedException)
      {
         threwResize = true;
      }
      
      Assert.True(threwResize);
   }

   [Fact]
   public void TransferReturnsArrayAndDisposes()
   {
      var pool = new CustomArrayPool<int>();
      var owner = new MemoryOwner<int>(5, false, pool);
      
      var array = owner.Transfer(out var length);
      
      Assert.NotNull(array);
      Assert.Equal(5, length);
      Assert.Equal(0, pool.ReturnCount);
      
      var threwLength = false;
      try
      {
         var _ = owner.Length;
      }
      catch (ObjectDisposedException)
      {
         threwLength = true;
      }
      
      Assert.True(threwLength);
      
      owner.Dispose();
      
      Assert.Equal(0, pool.ReturnCount);
   }

   [Fact]
   public void TransferOnDisposedThrowsObjectDisposedException()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Dispose();
      
      var threw = false;
      try
      {
         owner.Transfer(out _);
      }
      catch (ObjectDisposedException)
      {
         threw = true;
      }
      
      Assert.True(threw);
   }

   [Fact]
   public void TransferOnExternalArrayReturnsNullAndDisposes()
   {
      int[] array = [1, 2, 3, 4, 5];
      var owner = new MemoryOwner<int>(array);
      
      var returned = owner.Transfer(out var length);
      
      Assert.Same(array, returned);
      Assert.Equal(5, length);
      
      var threw = false;
      try
      {
         var _ = owner.Length;
      }
      catch (ObjectDisposedException)
      {
         threw = true;
      }
      
      Assert.True(threw);
   }

   [Fact]
   public void ImplicitOperatorsConvertSuccessfully()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner[0] = 123;
      
      var memory = (Memory<int>)owner;
      var readOnlyMemory = (ReadOnlyMemory<int>)owner;
      var span = (Span<int>)owner;
      var readOnlySpan = (ReadOnlySpan<int>)owner;
      
      Assert.Equal(123, memory.Span[0]);
      Assert.Equal(123, readOnlyMemory.Span[0]);
      Assert.Equal(123, span[0]);
      Assert.Equal(123, readOnlySpan[0]);
      
      owner.Dispose();
   }

   [Fact]
   public void ToStringDisposedStateReturnsExpectedString()
   {
      var owner = new MemoryOwner<int>(5);
      
      owner.Dispose();
      
      Assert.Equal("MemoryOwner(Disposed)", owner.ToString());
   }

   [Fact]
   public void ToStringExternalStateReturnsExpectedString()
   {
      int[] array = [1, 2, 3, 4, 5];
      var owner = new MemoryOwner<int>(array);
      
      Assert.Equal("MemoryOwner(Array)[5/5]", owner.ToString());
      
      owner.Dispose();
   }

   [Fact]
   public void ToStringPooledStateReturnsExpectedString()
   {
      var pool = new CustomArrayPool<int>();
      var owner = new MemoryOwner<int>(5, false, pool);
      
      var arraySegment = owner.DangerousGetArray();
      var bufferLength = arraySegment.Array!.Length;
      
      Assert.Equal($"MemoryOwner(Pooled)[5/{bufferLength}]", owner.ToString());
      
      owner.Dispose();
   }
}
