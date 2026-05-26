using System;
using Xunit;
using Beskar.Memory.Writers;
using Beskar.Memory.Owners;

namespace Beskar.Memory.Tests.Writers;

public class BufferWriterTests
{
   [Fact]
   public void ConstructorWithInitialSpan()
   {
      int[] buffer = [0, 0, 0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      Assert.Equal(5, writer.Capacity);
      Assert.Equal(5, writer.FreeCapacity);
      Assert.Equal(0, writer.Position);
      Assert.True(writer.WrittenSpan.IsEmpty);
      
      writer.Dispose();
   }

   [Fact]
   public void ConstructorWithRentedMemory()
   {
      var writer = new BufferWriter<int>(10);
      
      Assert.True(writer.Capacity >= 10);
      Assert.True(writer.FreeCapacity >= 10);
      Assert.Equal(0, writer.Position);
      Assert.True(writer.WrittenSpan.IsEmpty);
      
      writer.Dispose();
   }

   [Fact]
   public void FillFillsEntireActiveBuffer()
   {
      int[] buffer = [0, 0, 0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Fill(42);
      
      for (var i = 0; i < buffer.Length; i++)
      {
         Assert.Equal(42, buffer[i]);
      }
      
      writer.Dispose();
   }

   [Fact]
   public void AddByValueAndReference()
   {
      int[] buffer = [0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Add(10);
      var val = 20;
      writer.Add(ref val);
      
      Assert.Equal(2, writer.Position);
      Assert.Equal(1, writer.FreeCapacity);
      
      var written = writer.WrittenSpan;
      
      Assert.Equal(10, written[0]);
      Assert.Equal(20, written[1]);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteSpanBlittableAndReferenceTypes()
   {
      int[] buffer = [0, 0, 0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      int[] data = [10, 20, 30];
      
      writer.Write(data.AsSpan());
      
      Assert.Equal(3, writer.Position);
      Assert.Equal(2, writer.FreeCapacity);
      
      var written = writer.WrittenSpan;
      
      Assert.Equal(10, written[0]);
      Assert.Equal(20, written[1]);
      Assert.Equal(30, written[2]);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteReferenceTypesCorrectly()
   {
      string[] buffer = ["", "", ""];
      var writer = new BufferWriter<string>(buffer.AsSpan());
      string[] data = ["A", "B"];
      
      writer.Write(data.AsSpan());
      
      Assert.Equal(2, writer.Position);
      
      var written = writer.WrittenSpan;
      
      Assert.Equal("A", written[0]);
      Assert.Equal("B", written[1]);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteEmptySpanIsNoOp()
   {
      int[] buffer = [0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Write([]);
      
      Assert.Equal(0, writer.Position);
      
      writer.Dispose();
   }

   [Fact]
   public void AcquireSpanAndAdvance()
   {
      int[] buffer = [0, 0, 0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      var span = writer.AcquireSpan(3, true);
      span[0] = 10;
      span[1] = 20;
      span[2] = 30;
      
      Assert.Equal(3, writer.Position);
      
      var written = writer.WrittenSpan;
      
      Assert.Equal(10, written[0]);
      
      writer.Dispose();
   }

   [Fact]
   public void AcquireSpanWithoutMovingPosition()
   {
      int[] buffer = [0, 0, 0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      var span = writer.AcquireSpan(3, false);
      span[0] = 10;
      
      Assert.Equal(0, writer.Position);
      
      writer.Dispose();
   }

   [Fact]
   public void AdvanceAndAdvanceTo()
   {
      int[] buffer = [0, 0, 0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Advance(3);
      
      Assert.Equal(3, writer.Position);
      
      writer.AdvanceTo(5);
      
      Assert.Equal(5, writer.Position);
      
      writer.Dispose();
   }

   [Fact]
   public void MoveShiftsMemory()
   {
      int[] buffer = [1, 2, 3, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Advance(3);
      writer.Move(0, 3, 2, true);
      
      Assert.Equal(5, writer.Position);
      
      var written = writer.WrittenSpan;
      
      Assert.Equal(1, written[0]);
      Assert.Equal(2, written[1]);
      Assert.Equal(1, written[2]);
      Assert.Equal(2, written[3]);
      Assert.Equal(3, written[4]);
      
      writer.Dispose();
   }

   [Fact]
   public void GetMemoryOwnerThrowsIfNotGrown()
   {
      int[] buffer = [0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      var threw = false;
      try
      {
         var owner = writer.GetMemoryOwner();
      }
      catch (InvalidOperationException)
      {
         threw = true;
      }
      
      Assert.True(threw);
      
      writer.Dispose();
   }

   [Fact]
   public void GetMemoryOwnerReturnsCorrectOwnerAfterGrown()
   {
      int[] buffer = [0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Advance(5);
      var owner = writer.GetMemoryOwner();
      
      Assert.True(writer.Capacity >= 5);
      Assert.True(owner.Length >= 5);
      
      writer.Dispose();
   }

   [Fact]
   public void PositionSetterValidation()
   {
      int[] buffer = [0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Position = 2;
      
      Assert.Equal(2, writer.Position);
      
      var threwLower = false;
      try
      {
         writer.Position = -1;
      }
      catch (ArgumentOutOfRangeException)
      {
         threwLower = true;
      }
      
      Assert.True(threwLower);
      
      var threwUpper = false;
      try
      {
         writer.Position = 5;
      }
      catch (ArgumentOutOfRangeException)
      {
         threwUpper = true;
      }
      
      Assert.True(threwUpper);
      
      writer.Dispose();
   }

   [Fact]
   public void OperatorOverloads()
   {
      int[] buffer = [0, 0, 0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      int[] data = [2, 3];
      
      writer += 1;
      writer += data.AsSpan();
      writer <<= 4;
      writer <<= data.AsSpan();
      
      Assert.Equal(6, writer.Position);
      
      ReadOnlySpan<int> span = writer;
      
      Assert.Equal(1, span[0]);
      Assert.Equal(2, span[1]);
      Assert.Equal(3, span[2]);
      Assert.Equal(4, span[3]);
      Assert.Equal(2, span[4]);
      Assert.Equal(3, span[5]);
      
      writer.Dispose();
   }

   [Fact]
   public void AccessDisposedThrowsObjectDisposedException()
   {
      int[] buffer = [0, 0, 0];
      var writer = new BufferWriter<int>(buffer.AsSpan());
      
      writer.Dispose();
      
      var threwCapacity = false;
      try
      {
         var _ = writer.Capacity;
      }
      catch (ObjectDisposedException)
      {
         threwCapacity = true;
      }
      
      Assert.True(threwCapacity);
      
      var threwFree = false;
      try
      {
         var _ = writer.FreeCapacity;
      }
      catch (ObjectDisposedException)
      {
         threwFree = true;
      }
      
      Assert.True(threwFree);
      
      var threwSpan = false;
      try
      {
         var _ = writer.WrittenSpan;
      }
      catch (ObjectDisposedException)
      {
         threwSpan = true;
      }
      
      Assert.True(threwSpan);
      
      var threwPosGet = false;
      try
      {
         var _ = writer.Position;
      }
      catch (ObjectDisposedException)
      {
         threwPosGet = true;
      }
      
      Assert.True(threwPosGet);
      
      var threwPosSet = false;
      try
      {
         writer.Position = 1;
      }
      catch (ObjectDisposedException)
      {
         threwPosSet = true;
      }
      
      Assert.True(threwPosSet);
      
      var threwFill = false;
      try
      {
         writer.Fill(1);
      }
      catch (ObjectDisposedException)
      {
         threwFill = true;
      }
      
      Assert.True(threwFill);
      
      var threwAdd = false;
      try
      {
         writer.Add(1);
      }
      catch (ObjectDisposedException)
      {
         threwAdd = true;
      }
      
      Assert.True(threwAdd);
      
      var threwWrite = false;
      try
      {
         writer.Write([]);
      }
      catch (ObjectDisposedException)
      {
         threwWrite = true;
      }
      
      Assert.True(threwWrite);
      
      var threwAcquire = false;
      try
      {
         writer.AcquireSpan(1);
      }
      catch (ObjectDisposedException)
      {
         threwAcquire = true;
      }
      
      Assert.True(threwAcquire);
      
      var threwAdvance = false;
      try
      {
         writer.Advance(1);
      }
      catch (ObjectDisposedException)
      {
         threwAdvance = true;
      }
      
      Assert.True(threwAdvance);
      
      var threwAdvanceTo = false;
      try
      {
         writer.AdvanceTo(1);
      }
      catch (ObjectDisposedException)
      {
         threwAdvanceTo = true;
      }
      
      Assert.True(threwAdvanceTo);
      
      var threwMove = false;
      try
      {
         writer.Move(0, 0, 0);
      }
      catch (ObjectDisposedException)
      {
         threwMove = true;
      }
      
      Assert.True(threwMove);
      
      var threwOwner = false;
      try
      {
         writer.GetMemoryOwner();
      }
      catch (ObjectDisposedException)
      {
         threwOwner = true;
      }
      
      Assert.True(threwOwner);
   }
}
