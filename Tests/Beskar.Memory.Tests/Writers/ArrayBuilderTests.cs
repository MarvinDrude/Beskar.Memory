using System;
using Xunit;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Tests.Writers;

public class ArrayBuilderTests
{
   [Fact]
   public void ConstructorWithDefaults()
   {
      var builder = new ArrayBuilder<int>();
      
      Assert.Equal(0, builder.Count);
      Assert.True(builder.WrittenSpan.IsEmpty);
      Assert.Empty(builder.UnderlyingArray);
      
      builder.Dispose();
   }

   [Fact]
   public void AddByValueAndSpan()
   {
      var builder = new ArrayBuilder<int>(4);
      
      builder.Add(10);
      int[] data = [20, 30];
      builder.Add(data.AsSpan());
      
      Assert.Equal(3, builder.Count);
      
      var span = builder.WrittenSpan;
      
      Assert.Equal(10, span[0]);
      Assert.Equal(20, span[1]);
      Assert.Equal(30, span[2]);
      
      builder.Dispose();
   }

   [Fact]
   public void SetByIndex()
   {
      var builder = new ArrayBuilder<int>(4);
      
      builder.Add(10);
      builder.Set(0, 42);
      
      Assert.Equal(42, builder.WrittenSpan[0]);
      
      builder.Dispose();
   }

   [Fact]
   public void ClearReturnsToPool()
   {
      var builder = new ArrayBuilder<int>(4);
      
      builder.Add(10);
      builder.Clear();
      
      Assert.Equal(0, builder.Count);
      Assert.Empty(builder.UnderlyingArray);
      
      builder.Dispose();
   }

   [Fact]
   public void DisposeReturnsToPool()
   {
      var builder = new ArrayBuilder<int>(4);
      
      builder.Add(10);
      builder.Dispose();
      
      Assert.Throws<ObjectDisposedException>(() => builder.Count);
   }

   [Fact]
   public void AccessDisposedThrowsObjectDisposedException()
   {
      var builder = new ArrayBuilder<int>(4);
      
      builder.Dispose();
      
      Assert.Throws<ObjectDisposedException>(() => builder.Count);
      
      var threwSpan = false;
      try
      {
         var _ = builder.WrittenSpan;
      }
      catch (ObjectDisposedException)
      {
         threwSpan = true;
      }
      Assert.True(threwSpan);
      
      Assert.Throws<ObjectDisposedException>(() => builder.UnderlyingArray);
      Assert.Throws<ObjectDisposedException>(() => builder.Add(1));
      Assert.Throws<ObjectDisposedException>(() => builder.Add(ReadOnlySpan<int>.Empty));
      Assert.Throws<ObjectDisposedException>(() => builder.Set(0, 1));
      Assert.Throws<ObjectDisposedException>(() => builder.Clear());
   }

   [Fact]
   public void ConstructorResultWithInitialSpan()
   {
      var builder = new ArrayBuilder<int>(4);
      builder.Add(10);
      builder.Add(20);
      
      var result = new ArrayBuilderResult<int>(builder);
      
      Assert.True(result.HasValue);
      Assert.Equal(2, result.WrittenSpan.Length);
      Assert.Equal(10, result.WrittenSpan[0]);
      Assert.Equal(20, result.WrittenSpan[1]);
      
      result.Dispose();
   }

   [Fact]
   public void ConstructorResultWithEmpty()
   {
      var result = ArrayBuilderResult<int>.Empty;
      
      Assert.False(result.HasValue);
      Assert.True(result.WrittenSpan.IsEmpty);
      
      result.Dispose();
   }

   [Fact]
   public void ResultImplicitOperators()
   {
      var builder = new ArrayBuilder<int>(4);
      builder.Add(42);
      
      ArrayBuilderResult<int> result = builder;
      
      Assert.True(result.HasValue);
      
      Span<int> span = result;
      ReadOnlySpan<int> readOnlySpan = result;
      
      Assert.Equal(1, span.Length);
      Assert.Equal(42, span[0]);
      Assert.Equal(1, readOnlySpan.Length);
      Assert.Equal(42, readOnlySpan[0]);
      
      result.Dispose();
   }

   [Fact]
   public void ResultDisposalSafety()
   {
      var builder = new ArrayBuilder<int>(4);
      builder.Add(10);
      
      var result = new ArrayBuilderResult<int>(builder);
      result.Dispose();
      
      var threwSpan = false;
      try
      {
         var _ = result.WrittenSpan;
      }
      catch (ObjectDisposedException)
      {
         threwSpan = true;
      }
      Assert.True(threwSpan);
   }
}
