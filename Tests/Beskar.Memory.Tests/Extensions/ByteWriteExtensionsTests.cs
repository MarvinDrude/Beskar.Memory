using System;
using Xunit;
using Beskar.Memory.Extensions;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Tests.Extensions;

public class ByteWriteExtensionsTests
{
   [Fact]
   public void WriteLittleEndianToSpan()
   {
      var buffer = new byte[8];
      
      var val16 = (short)0x0201;
      var written16 = val16.WriteLittleEndian(buffer.AsSpan());
      Assert.Equal(2, written16);
      Assert.Equal(0x01, buffer[0]);
      Assert.Equal(0x02, buffer[1]);

      var val32 = 0x04030201;
      var written32 = val32.WriteLittleEndian(buffer.AsSpan());
      Assert.Equal(4, written32);
      Assert.Equal(0x01, buffer[0]);
      Assert.Equal(0x02, buffer[1]);
      Assert.Equal(0x03, buffer[2]);
      Assert.Equal(0x04, buffer[3]);

      var val64 = 0x0807060504030201;
      var written64 = val64.WriteLittleEndian(buffer.AsSpan());
      Assert.Equal(8, written64);
      Assert.Equal(0x01, buffer[0]);
      Assert.Equal(0x02, buffer[1]);
      Assert.Equal(0x03, buffer[2]);
      Assert.Equal(0x04, buffer[3]);
      Assert.Equal(0x05, buffer[4]);
      Assert.Equal(0x06, buffer[5]);
      Assert.Equal(0x07, buffer[6]);
      Assert.Equal(0x08, buffer[7]);
   }

   [Fact]
   public void WriteBigEndianToSpan()
   {
      var buffer = new byte[8];
      
      var val16 = (short)0x0102;
      var written16 = val16.WriteBigEndian(buffer.AsSpan());
      Assert.Equal(2, written16);
      Assert.Equal(0x01, buffer[0]);
      Assert.Equal(0x02, buffer[1]);

      var val32 = 0x01020304;
      var written32 = val32.WriteBigEndian(buffer.AsSpan());
      Assert.Equal(4, written32);
      Assert.Equal(0x01, buffer[0]);
      Assert.Equal(0x02, buffer[1]);
      Assert.Equal(0x03, buffer[2]);
      Assert.Equal(0x04, buffer[3]);

      var val64 = 0x0102030405060708;
      var written64 = val64.WriteBigEndian(buffer.AsSpan());
      Assert.Equal(8, written64);
      Assert.Equal(0x01, buffer[0]);
      Assert.Equal(0x02, buffer[1]);
      Assert.Equal(0x03, buffer[2]);
      Assert.Equal(0x04, buffer[3]);
      Assert.Equal(0x05, buffer[4]);
      Assert.Equal(0x06, buffer[5]);
      Assert.Equal(0x07, buffer[6]);
      Assert.Equal(0x08, buffer[7]);
   }

   [Fact]
   public void WriteThrowsWhenSpanTooSmall()
   {
      var buffer = new byte[2];
      var val32 = 42;

      Assert.Throws<ArgumentOutOfRangeException>(() => val32.WriteLittleEndian(buffer.AsSpan()));
      Assert.Throws<ArgumentOutOfRangeException>(() => val32.WriteBigEndian(buffer.AsSpan()));
   }

   [Fact]
   public void WriteToBufferWriter()
   {
      Span<byte> initialSpan = new byte[16];
      var writer = new BufferWriter<byte>(initialSpan);

      var val32 = 0x04030201;
      var written32 = val32.WriteLittleEndian(ref writer);
      Assert.Equal(4, written32);

      var val64 = 0x0102030405060708;
      var written64 = val64.WriteBigEndian(ref writer);
      Assert.Equal(8, written64);

      var writtenSpan = writer.WrittenSpan;
      Assert.Equal(12, writtenSpan.Length);

      Assert.Equal(0x01, writtenSpan[0]);
      Assert.Equal(0x02, writtenSpan[1]);
      Assert.Equal(0x03, writtenSpan[2]);
      Assert.Equal(0x04, writtenSpan[3]);

      Assert.Equal(0x01, writtenSpan[4]);
      Assert.Equal(0x02, writtenSpan[5]);
      Assert.Equal(0x03, writtenSpan[6]);
      Assert.Equal(0x04, writtenSpan[7]);
      Assert.Equal(0x05, writtenSpan[8]);
      Assert.Equal(0x06, writtenSpan[9]);
      Assert.Equal(0x07, writtenSpan[10]);
      Assert.Equal(0x08, writtenSpan[11]);

      writer.Dispose();
   }
}
