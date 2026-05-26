using System;
using Xunit;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Tests.Collections;

public class SequenceArrayTests
{
   [Fact]
   public void ConstructorAndProperties()
   {
      var defaultArray = new SequenceArray<int>();
      Assert.Equal(0, defaultArray.Length);
      Assert.Empty(defaultArray);
      Assert.True(defaultArray.Span.IsEmpty);
      
      int[] data = [10, 20, 30];
      var array = new SequenceArray<int>(data);
      
      Assert.Equal(3, array.Length);
      Assert.Equal(3, array.Count);
      Assert.Equal(10, array[0]);
      Assert.Equal(30, array[2]);
      
      var arrayFromSpan = new SequenceArray<int>(data.AsSpan());
      Assert.Equal(3, arrayFromSpan.Length);
      Assert.Equal(20, arrayFromSpan[1]);
   }

   [Fact]
   public void CollectionExpressionSupport()
   {
      SequenceArray<int> array = [1, 2, 3, 4];
      
      Assert.Equal(4, array.Length);
      Assert.Equal(1, array[0]);
      Assert.Equal(4, array[3]);
   }

   [Fact]
   public void SequenceEquality()
   {
      SequenceArray<int> array1 = [10, 20, 30];
      SequenceArray<int> array2 = [10, 20, 30];
      SequenceArray<int> array3 = [10, 20, 40];
      
      Assert.True(array1.Equals(array2));
      Assert.True(array1 == array2);
      Assert.False(array1 == array3);
      Assert.False(array1 != array2);
      Assert.True(array1 != array3);
      
      Assert.True(array1.Equals((object)array2));
      Assert.False(array1.Equals("NotAnArray"));
      
      Assert.Equal(array1.GetHashCode(), array2.GetHashCode());
      Assert.NotEqual(array1.GetHashCode(), array3.GetHashCode());
   }

   [Fact]
   public void RangeIndexing()
   {
      SequenceArray<int> array = [10, 20, 30, 40, 50];
      
      Assert.Equal(50, array[^1]);
      
      var slice = array[1..4];
      
      Assert.Equal(3, slice.Length);
      Assert.Equal(20, slice[0]);
      Assert.Equal(40, slice[2]);
   }

   [Fact]
   public void ImplicitCastOperators()
   {
      SequenceArray<int> array = [10, 20];
      
      ReadOnlySpan<int> readOnlySpan = array;
      Span<int> span = array;
      Memory<int> memory = array;
      ReadOnlyMemory<int> readOnlyMemory = array;
      
      Assert.Equal(2, readOnlySpan.Length);
      Assert.Equal(10, span[0]);
      Assert.Equal(20, memory.Span[1]);
      Assert.Equal(2, readOnlyMemory.Length);
      
      int[] data = [1, 2, 3];
      SequenceArray<int> arrayFromRaw = data;
      
      Assert.Equal(3, arrayFromRaw.Length);
      Assert.Equal(2, arrayFromRaw[1]);
   }
}
