using System;
using Xunit;
using Beskar.Memory.Buffers;

namespace Beskar.Memory.Tests.Buffers;

public class CircularBufferTests
{
   [Fact]
   public void TwoSpanOperations()
   {
      Span<int> first = [10, 20];
      Span<int> second = [30, 40, 50];

      var twoSpan = new TwoSpan<int>(first, second);

      Assert.Equal(5, twoSpan.Length);
      Assert.Equal(10, twoSpan[0]);
      Assert.Equal(30, twoSpan[2]);
      Assert.Equal(50, twoSpan[^1]);

      var dest = new int[5];
      twoSpan.CopyTo(dest);
      Assert.Equal([10, 20, 30, 40, 50], dest);

      var tryDest = new int[5];
      Assert.True(twoSpan.TryCopyTo(tryDest));
      Assert.Equal([10, 20, 30, 40, 50], tryDest);

      var tooShort = new int[4];
      Assert.False(twoSpan.TryCopyTo(tooShort));

      var array = twoSpan.ToArray();
      Assert.Equal([10, 20, 30, 40, 50], array);

      var sum = 0;
      foreach (ref var val in twoSpan)
      {
         sum += val;
      }
      Assert.Equal(150, sum);
   }

   [Fact]
   public void CircularBufferAddAndIndexer()
   {
      Span<int> backing = new int[3];
      var buffer = new CircularBufferSlim<int>(backing);

      Assert.Equal(3, buffer.Capacity);
      Assert.Equal(0, buffer.Count);

      buffer.Add(10);
      buffer.Add(20);

      Assert.Equal(2, buffer.Count);
      Assert.Equal(10, buffer[0]);
      Assert.Equal(20, buffer[1]);

      var threw = false;
      try
      {
         var _ = buffer[2];
      }
      catch (ArgumentOutOfRangeException)
      {
         threw = true;
      }
      Assert.True(threw);
   }

   [Fact]
   public void CircularBufferWrapAround()
   {
      Span<int> backing = new int[3];
      var buffer = new CircularBufferSlim<int>(backing);

      buffer.Add(10);
      buffer.Add(20);
      buffer.Add(30);
      buffer.Add(40);

      Assert.Equal(3, buffer.Count);
      Assert.Equal(20, buffer[0]);
      Assert.Equal(30, buffer[1]);
      Assert.Equal(40, buffer[2]);

      var split = buffer.WrittenTwoSpan;
      Assert.Equal(2, split.First.Length);
      Assert.Equal(20, split.First[0]);
      Assert.Equal(30, split.First[1]);
      Assert.Equal(1, split.Second.Length);
      Assert.Equal(40, split.Second[0]);
   }

   [Fact]
   public void CircularBufferClearAndQueue()
   {
      Span<int> backing = new int[3];
      var buffer = new CircularBufferSlim<int>(backing);

      buffer.Add(1);
      buffer.Add(2);

      Assert.True(buffer.TryDequeue(out var first));
      Assert.Equal(1, first);
      Assert.Equal(1, buffer.Count);

      buffer.Add(3);
      buffer.Add(4);

      Assert.Equal(2, buffer.Dequeue());
      Assert.Equal(3, buffer.Dequeue());
      Assert.Equal(4, buffer.Dequeue());
      Assert.Equal(0, buffer.Count);

      var threw = false;
      try
      {
         buffer.Dequeue();
      }
      catch (InvalidOperationException)
      {
         threw = true;
      }
      Assert.True(threw);

      buffer.Add(100);
      buffer.Clear();
      Assert.Equal(0, buffer.Count);
   }

   [Fact]
   public void ClassBufferAddAndIndexer()
   {
      var buffer = new CircularBuffer<int>(3);

      Assert.Equal(3, buffer.Capacity);
      Assert.Equal(0, buffer.Count);

      buffer.Add(10);
      buffer.Add(20);

      Assert.Equal(2, buffer.Count);
      Assert.Equal(10, buffer[0]);
      Assert.Equal(20, buffer[1]);

      var threw = false;
      try
      {
         var _ = buffer[2];
      }
      catch (ArgumentOutOfRangeException)
      {
         threw = true;
      }
      Assert.True(threw);

      buffer.Dispose();
   }

   [Fact]
   public void ClassBufferWrapAround()
   {
      var buffer = new CircularBuffer<int>(3);

      buffer.Add(10);
      buffer.Add(20);
      buffer.Add(30);
      buffer.Add(40);

      Assert.Equal(3, buffer.Count);
      Assert.Equal(20, buffer[0]);
      Assert.Equal(30, buffer[1]);
      Assert.Equal(40, buffer[2]);

      var split = buffer.WrittenTwoSpan;
      Assert.Equal(2, split.First.Length);
      Assert.Equal(20, split.First[0]);
      Assert.Equal(30, split.First[1]);
      Assert.Equal(1, split.Second.Length);
      Assert.Equal(40, split.Second[0]);

      buffer.Dispose();
   }

   [Fact]
   public void ClassBufferClearAndQueue()
   {
      var buffer = new CircularBuffer<int>(3);

      buffer.Add(1);
      buffer.Add(2);

      Assert.True(buffer.TryDequeue(out var first));
      Assert.Equal(1, first);
      Assert.Equal(1, buffer.Count);

      buffer.Add(3);
      buffer.Add(4);

      Assert.Equal(2, buffer.Dequeue());
      Assert.Equal(3, buffer.Dequeue());
      Assert.Equal(4, buffer.Dequeue());
      Assert.Equal(0, buffer.Count);

      var threw = false;
      try
      {
         buffer.Dequeue();
      }
      catch (InvalidOperationException)
      {
         threw = true;
      }
      Assert.True(threw);

      buffer.Add(100);
      buffer.Clear();
      Assert.Equal(0, buffer.Count);

      buffer.Dispose();
   }

   [Fact]
   public void ClassBufferDisposalSafety()
   {
      var buffer = new CircularBuffer<int>(3);
      buffer.Dispose();

      Assert.Throws<ObjectDisposedException>(() => buffer.Capacity);
      Assert.Throws<ObjectDisposedException>(() => buffer.Add(1));
      Assert.Throws<ObjectDisposedException>(() => buffer.Clear());

      var threwBuffer = false;
      try
      {
         var _ = buffer.Buffer;
      }
      catch (ObjectDisposedException)
      {
         threwBuffer = true;
      }
      Assert.True(threwBuffer);

      var threwSpan = false;
      try
      {
         var _ = buffer.WrittenTwoSpan;
      }
      catch (ObjectDisposedException)
      {
         threwSpan = true;
      }
      Assert.True(threwSpan);

      var threwIndexer = false;
      try
      {
         var _ = buffer[0];
      }
      catch (ObjectDisposedException)
      {
         threwIndexer = true;
      }
      Assert.True(threwIndexer);
   }

   [Fact]
   public void CircularBufferMemoryLeakTest()
   {
      var buffer = new CircularBuffer<object>(3);
      var item = new object();
      var weakRef = new WeakReference(item);

      buffer.Add(item);
      Assert.True(buffer.TryDequeue(out var dequeued));
      Assert.Same(item, dequeued);

      // Clear the local references
      item = null;
      dequeued = null;

      // Force GC
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      // Assert that the object has been collected because the circular buffer no longer holds a reference to it
      Assert.False(weakRef.IsAlive);
      buffer.Dispose();
   }

   [Fact]
   public void CircularBufferSlimMemoryLeakTest()
   {
      var backing = new object[3];
      var buffer = new CircularBufferSlim<object>(backing);
      var item = new object();
      var weakRef = new WeakReference(item);

      buffer.Add(item);
      Assert.True(buffer.TryDequeue(out var dequeued));
      Assert.Same(item, dequeued);

      // Clear local references
      item = null;
      dequeued = null;

      // Dequeue should have cleared the backing element
      Assert.Null(backing[0]);

      // Force GC
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      // Assert that the object has been collected
      Assert.False(weakRef.IsAlive);
   }
}
