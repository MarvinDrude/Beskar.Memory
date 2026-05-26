using System;
using System.Buffers;
using System.Text;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Tests.Buffers;

public class ByteReaderTests
{
   [Fact]
   public void ReadContiguousBytes()
   {
      Span<byte> backing = new byte[256];
      var writer = new ByteWriter(backing);
      
      writer.WriteLittleEndian<ushort>(42);
      writer.WriteBigEndian<uint>(100);
      writer.WriteLittleEndian<ulong>(123456789UL);
      writer.WriteString("Hello UTF8");
      
      var rawStringBytes = writer.WriteStringRaw("RawString");
      
      var writtenBytes = writer.WrittenSpan;
      var reader = new ByteReader(writtenBytes);
      
      Assert.Equal(42, reader.ReadLittleEndian<ushort>());
      Assert.Equal(100U, reader.ReadBigEndian<uint>());
      Assert.Equal(123456789UL, reader.ReadLittleEndian<ulong>());
      Assert.Equal("Hello UTF8", reader.ReadString(10, Encoding.UTF8));
      
      Assert.Equal("RawString", reader.ReadStringRawToString(rawStringBytes));
      
      writer.Dispose();
   }

   [Fact]
   public void ReadBuiltInTypes()
   {
      Span<byte> backing = new byte[256];
      var writer = new ByteWriter(backing);
      
      writer.WriteLittleEndian<sbyte>(-12);
      writer.WriteLittleEndian<byte>(240);
      writer.WriteLittleEndian<short>(-3000);
      writer.WriteLittleEndian<ushort>(60000);
      writer.WriteLittleEndian<int>(-2000000);
      writer.WriteLittleEndian<uint>(4000000000U);
      writer.WriteLittleEndian<long>(-9000000000000L);
      writer.WriteLittleEndian<ulong>(18000000000000000000UL);
      writer.WriteLittleEndian<char>('Ω');
      writer.WriteLittleEndian<float>(3.14F);
      writer.WriteLittleEndian<double>(2.71828);
      writer.WriteLittleEndian<bool>(true);
      writer.WriteLittleEndian<Half>((Half)1.5f);

      writer.WriteBigEndian<sbyte>(-12);
      writer.WriteBigEndian<byte>(240);
      writer.WriteBigEndian<short>(-3000);
      writer.WriteBigEndian<ushort>(60000);
      writer.WriteBigEndian<int>(-2000000);
      writer.WriteBigEndian<uint>(4000000000U);
      writer.WriteBigEndian<long>(-9000000000000L);
      writer.WriteBigEndian<ulong>(18000000000000000000UL);
      writer.WriteBigEndian<char>('Ω');
      writer.WriteBigEndian<float>(3.14F);
      writer.WriteBigEndian<double>(2.71828);
      writer.WriteBigEndian<bool>(true);
      writer.WriteBigEndian<Half>((Half)1.5f);

      var writtenBytes = writer.WrittenSpan;
      var reader = new ByteReader(writtenBytes);

      Assert.Equal((sbyte)-12, reader.ReadLittleEndian<sbyte>());
      Assert.Equal((byte)240, reader.ReadLittleEndian<byte>());
      Assert.Equal((short)-3000, reader.ReadLittleEndian<short>());
      Assert.Equal((ushort)60000, reader.ReadLittleEndian<ushort>());
      Assert.Equal(-2000000, reader.ReadLittleEndian<int>());
      Assert.Equal(4000000000U, reader.ReadLittleEndian<uint>());
      Assert.Equal(-9000000000000L, reader.ReadLittleEndian<long>());
      Assert.Equal(18000000000000000000UL, reader.ReadLittleEndian<ulong>());
      Assert.Equal('Ω', reader.ReadLittleEndian<char>());
      Assert.Equal(3.14F, reader.ReadLittleEndian<float>());
      Assert.Equal(2.71828, reader.ReadLittleEndian<double>());
      Assert.True(reader.ReadLittleEndian<bool>());
      Assert.Equal((Half)1.5f, reader.ReadLittleEndian<Half>());

      Assert.Equal((sbyte)-12, reader.ReadBigEndian<sbyte>());
      Assert.Equal((byte)240, reader.ReadBigEndian<byte>());
      Assert.Equal((short)-3000, reader.ReadBigEndian<short>());
      Assert.Equal((ushort)60000, reader.ReadBigEndian<ushort>());
      Assert.Equal(-2000000, reader.ReadBigEndian<int>());
      Assert.Equal(4000000000U, reader.ReadBigEndian<uint>());
      Assert.Equal(-9000000000000L, reader.ReadBigEndian<long>());
      Assert.Equal(18000000000000000000UL, reader.ReadBigEndian<ulong>());
      Assert.Equal('Ω', reader.ReadBigEndian<char>());
      Assert.Equal(3.14F, reader.ReadBigEndian<float>());
      Assert.Equal(2.71828, reader.ReadBigEndian<double>());
      Assert.True(reader.ReadBigEndian<bool>());
      Assert.Equal((Half)1.5f, reader.ReadBigEndian<Half>());

      writer.Dispose();
   }

   [Fact]
   public void ReadSequenceBytes()
   {
      var firstSegment = new byte[] { 10, 20 };
      var secondSegment = new byte[] { 30, 40 };
      
      var segment1 = new BufferSegment(firstSegment);
      var segment2 = new BufferSegment(secondSegment);
      segment1.SetNext(segment2);
      
      var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, 2);
      var reader = new ByteReader(sequence);
      
      Assert.Equal(4, reader.BytesRemaining);
      
      var bytes = reader.ReadBytes(4);
      Assert.Equal(4, bytes.Length);
      Assert.Equal(10, bytes[0]);
      Assert.Equal(40, bytes[3]);
      
      Assert.Equal(0, reader.BytesRemaining);
   }

   [Fact]
   public void PositionAndRemaining()
   {
      Span<byte> buffer = [1, 2, 3, 4, 5];
      var reader = new ByteReader(buffer);
      
      Assert.Equal(5, reader.BytesRemaining);
      Assert.Equal(0, reader.Position);
      
      Assert.Equal(1, reader.ReadByte());
      Assert.Equal(4, reader.BytesRemaining);
      Assert.Equal(1, reader.Position);
      
      reader.Position = 3;
      Assert.Equal(2, reader.BytesRemaining);
      Assert.Equal(4, reader.ReadByte());
   }

   [Fact]
   public void ReadStringFromSequenceWithSplit()
   {
      byte[] firstSegment = [72, 101, 108, 108, 111, 32];
      byte[] secondSegment = [87, 111, 114, 108, 100, 33];
      
      var segment1 = new BufferSegment(firstSegment);
      var segment2 = new BufferSegment(secondSegment);
      segment1.SetNext(segment2);
      
      var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, secondSegment.Length);
      var reader = new ByteReader(sequence);
      
      var result = reader.ReadString(12, Encoding.UTF8);
      Assert.Equal("Hello World!", result);
   }

   [Fact]
   public void ReadSingleBytes()
   {
      Span<byte> buffer = [1, 2, 3, 4];
      var reader = new ByteReader(buffer);
      
      Assert.Equal(1, reader.ReadByte());
      Assert.Equal(2, reader.ReadByte());
      
      byte[] firstSegment = [5];
      byte[] secondSegment = [6];
      var segment1 = new BufferSegment(firstSegment);
      var segment2 = new BufferSegment(secondSegment);
      segment1.SetNext(segment2);
      
      var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, 1);
      var readerSeq = new ByteReader(sequence);
      
      Assert.Equal(5, readerSeq.ReadByte());
      Assert.Equal(6, readerSeq.ReadByte());
   }

   [Fact]
   public void ReadBytesFromSequence()
   {
      byte[] firstSegment = [1, 2, 3];
      byte[] secondSegment = [4, 5, 6];
      var segment1 = new BufferSegment(firstSegment);
      var segment2 = new BufferSegment(secondSegment);
      segment1.SetNext(segment2);
      
      var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, 3);
      var reader = new ByteReader(sequence);
      
      var firstPart = reader.ReadBytes(2);
      Assert.Equal(2, firstPart.Length);
      Assert.Equal(1, firstPart[0]);
      Assert.Equal(2, firstPart[1]);
      
      var secondPart = reader.ReadBytes(3);
      Assert.Equal(3, secondPart.Length);
      Assert.Equal(3, secondPart[0]);
      Assert.Equal(4, secondPart[1]);
      Assert.Equal(5, secondPart[2]);
   }

   [Fact]
   public void ReadStringRawRecasting()
   {
      Span<byte> buffer = [65, 0, 66, 0];
      var reader = new ByteReader(buffer);
      
      var chars = reader.ReadStringRaw(4);
      Assert.Equal(2, chars.Length);
      Assert.Equal('A', chars[0]);
      Assert.Equal('B', chars[1]);
      
      reader.Position = 0;
      var str = reader.ReadStringRawToString(4);
      Assert.Equal("AB", str);
   }

   [Fact]
   public void ErrorsAndOutOfBounds()
   {
      Span<byte> buffer = [1, 2, 3];
      var reader = new ByteReader(buffer);
      
      try
      {
         reader.Position = -1;
         Assert.Fail("Should have thrown ArgumentOutOfRangeException");
      }
      catch (ArgumentOutOfRangeException)
      {
      }

      try
      {
         reader.Position = 4;
         Assert.Fail("Should have thrown ArgumentOutOfRangeException");
      }
      catch (ArgumentOutOfRangeException)
      {
      }
      
      reader.Position = 3;

      try
      {
         reader.ReadByte();
         Assert.Fail("Should have thrown IndexOutOfRangeException");
      }
      catch (IndexOutOfRangeException)
      {
      }

      try
      {
         reader.ReadBytes(1);
         Assert.Fail("Should have thrown ArgumentOutOfRangeException");
      }
      catch (ArgumentOutOfRangeException)
      {
      }
      
      byte[] firstSegment = [1];
      var segment1 = new BufferSegment(firstSegment);
      var sequence = new ReadOnlySequence<byte>(segment1, 0, segment1, 1);
      var readerSeq = new ByteReader(sequence);
      
      readerSeq.ReadByte();

      try
      {
         readerSeq.ReadByte();
         Assert.Fail("Should have thrown InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }
   }

   [Fact]
   public void MultiByteEndiannessSegmentBoundary()
   {
      byte[] firstSegment = [0x00, 0x00, 0x01];
      byte[] secondSegment = [0x02, 0x03, 0x04];
      var segment1 = new BufferSegment(firstSegment);
      var segment2 = new BufferSegment(secondSegment);
      segment1.SetNext(segment2);
      
      var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, 3);
      var reader = new ByteReader(sequence);
      
      Assert.Equal(0x02010000U, reader.ReadLittleEndian<uint>());
   }

   private sealed class BufferSegment : ReadOnlySequenceSegment<byte>
   {
      public BufferSegment(Memory<byte> memory)
      {
         Memory = memory;
      }

      public void SetNext(BufferSegment next)
      {
         Next = next;
         next.RunningIndex = RunningIndex + Memory.Length;
      }
   }
}
