using System;
using System.Buffers.Binary;
using System.Text;
using Xunit;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Tests.Writers;

public class ByteWriterTests
{
   [Fact]
   public void ConstructorWithInitialSpan()
   {
      byte[] buffer = [0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      
      Assert.Equal(5, writer.Capacity);
      Assert.Equal(0, writer.Position);
      Assert.True(writer.WrittenSpan.IsEmpty);
      
      writer.Dispose();
   }

   [Fact]
   public void ConstructorWithRentedMemory()
   {
      var writer = new ByteWriter(10);
      
      Assert.True(writer.Capacity >= 10);
      Assert.Equal(0, writer.Position);
      Assert.True(writer.WrittenSpan.IsEmpty);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteBigEndianPrimitives()
   {
      byte[] buffer = [0, 0, 0, 0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      
      var size = writer.WriteBigEndian((int)0x12345678);
      
      Assert.Equal(4, size);
      Assert.Equal(4, writer.Position);
      
      var written = writer.WrittenSpan;
      var val = BinaryPrimitives.ReadInt32BigEndian(written[..4]);
      
      Assert.Equal(0x12345678, val);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteLittleEndianPrimitives()
   {
      byte[] buffer = [0, 0, 0, 0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      
      var size = writer.WriteLittleEndian((int)0x12345678);
      
      Assert.Equal(4, size);
      Assert.Equal(4, writer.Position);
      
      var written = writer.WrittenSpan;
      var val = BinaryPrimitives.ReadInt32LittleEndian(written[..4]);
      
      Assert.Equal(0x12345678, val);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteStringRawRecastsMemory()
   {
      byte[] buffer = [0, 0, 0, 0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      
      var size = writer.WriteStringRaw("AB".AsSpan());
      
      Assert.Equal(4, size);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteStringUtf8AndCustomEncoding()
   {
      byte[] buffer = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      
      var sizeUtf8 = writer.WriteString("ABC".AsSpan());
      
      Assert.Equal(3, sizeUtf8);
      
      var sizeUnicode = writer.WriteString("XY".AsSpan(), Encoding.Unicode);
      
      Assert.Equal(4, sizeUnicode);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteByteAndBytes()
   {
      byte[] buffer = [0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      byte[] data = [2, 3];
      
      writer.WriteByte(1);
      writer.WriteBytes(data.AsSpan());
      
      Assert.Equal(3, writer.Position);
      
      var written = writer.WrittenSpan;
      
      Assert.Equal(1, written[0]);
      Assert.Equal(2, written[1]);
      Assert.Equal(3, written[2]);
      
      writer.Dispose();
   }

   [Fact]
   public void FillFillsEntireBuffer()
   {
      byte[] buffer = [0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      
      writer.Fill(42);
      
      var written = writer.WrittenSpan;
      for (var i = 0; i < written.Length; i++)
      {
         Assert.Equal(42, written[i]);
      }
      
      writer.Dispose();
   }

   [Fact]
   public void OperatorOverloads()
   {
      byte[] buffer = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      byte[] data = [0xBB, 0xCC];
      
      writer += (byte)0xAA;
      writer += data.AsSpan();
      writer += "XY".AsSpan();
      
      writer <<= (byte)0x11;
      writer <<= data.AsSpan();
      writer <<= "ZW".AsSpan();
      
      Assert.Equal(10, writer.Position);
      
      ReadOnlySpan<byte> span = writer;
      
      Assert.Equal(0xAA, span[0]);
      Assert.Equal(0xBB, span[1]);
      Assert.Equal(0xCC, span[2]);
      Assert.Equal((byte)'X', span[3]);
      Assert.Equal((byte)'Y', span[4]);
      Assert.Equal(0x11, span[5]);
      Assert.Equal(0xBB, span[6]);
      Assert.Equal(0xCC, span[7]);
      Assert.Equal((byte)'Z', span[8]);
      Assert.Equal((byte)'W', span[9]);
      
      writer.Dispose();
   }

   [Fact]
   public void AccessDisposedThrowsObjectDisposedException()
   {
      byte[] buffer = [0, 0, 0];
      var writer = new ByteWriter(buffer.AsSpan());
      
      writer.Dispose();
      
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
      
      var threwWriteByte = false;
      try
      {
         writer.WriteByte(1);
      }
      catch (ObjectDisposedException)
      {
         threwWriteByte = true;
      }
      
      Assert.True(threwWriteByte);
   }
}
