using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Serializers;

namespace Beskar.Memory.Serialization.Tests;

public class VarIntegerTests
{
   #region Uint32 Tests

   [Theory]
   [InlineData(0u, 1)]
   [InlineData(1u, 1)]
   [InlineData(127u, 1)]
   [InlineData(128u, 2)]
   [InlineData(16383u, 2)]
   [InlineData(16384u, 3)]
   [InlineData(2097151u, 3)]
   [InlineData(2097152u, 4)]
   [InlineData(268435455u, 4)]
   [InlineData(268435456u, 5)]
   [InlineData(uint.MaxValue, 5)]
   public void TestUint32RoundTripAndLength(uint value, int expectedLength)
   {
      // 1. Calculate length
      var calculatedLength = VarInteger.CalculateByteLength(value);
      Assert.Equal(expectedLength, calculatedLength);

      // 2. Write
      Span<byte> buffer = stackalloc byte[calculatedLength];
      var writer = new BufferWriter<byte>(buffer);
      var written = VarInteger.Write(ref writer, value);
      Assert.Equal(expectedLength, written);
      Assert.Equal(expectedLength, writer.Position);

      // 3. Read
      var sequence = new ReadOnlySequence<byte>(buffer.ToArray());
      var reader = new SequenceReader<byte>(sequence);
      Assert.True(VarInteger.TryRead(ref reader, out uint result));
      Assert.Equal(value, result);
      Assert.Equal(expectedLength, reader.Consumed);
   }

   [Fact]
   public void TestUint32RandomValues()
   {
      var rand = new Random(42);
      Span<byte> buffer = stackalloc byte[5];

      for (int i = 0; i < 10000; i++)
      {
         var value = (uint)rand.Next();
         if (rand.NextDouble() > 0.5)
         {
            // Occasionally generate fully distributed uints
            value = (value << 16) | (uint)rand.Next(0, 65536);
         }

         var calculatedLength = VarInteger.CalculateByteLength(value);
         var writer = new BufferWriter<byte>(buffer);
         VarInteger.Write(ref writer, value);
         Assert.Equal(calculatedLength, writer.Position);

         var sequence = new ReadOnlySequence<byte>(buffer[..calculatedLength].ToArray());
         var reader = new SequenceReader<byte>(sequence);
         Assert.True(VarInteger.TryRead(ref reader, out uint result));
         Assert.Equal(value, result);
      }
   }

   #endregion

   #region Int32 Tests

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(-1, 1)]
    [InlineData(63, 1)]
    [InlineData(-63, 1)]
    [InlineData(64, 2)]
    [InlineData(-64, 1)]
    [InlineData(-65, 2)]
    [InlineData(8191, 2)]
    [InlineData(-8191, 2)]
    [InlineData(8192, 3)]
    [InlineData(-8192, 2)]
    [InlineData(-8193, 3)]
    [InlineData(1048575, 3)]
    [InlineData(-1048575, 3)]
    [InlineData(1048576, 4)]
    [InlineData(-1048576, 3)]
    [InlineData(-1048577, 4)]
    [InlineData(134217727, 4)]
    [InlineData(-134217727, 4)]
    [InlineData(134217728, 5)]
    [InlineData(-134217728, 4)]
    [InlineData(-134217729, 5)]
    [InlineData(int.MaxValue, 5)]
    [InlineData(int.MinValue, 5)]
    public void TestInt32RoundTripAndLength(int value, int expectedLength)
   {
      // 1. Calculate length
      var calculatedLength = VarInteger.CalculateByteLength(value);
      Assert.Equal(expectedLength, calculatedLength);

      // 2. Write
      Span<byte> buffer = stackalloc byte[calculatedLength];
      var writer = new BufferWriter<byte>(buffer);
      var written = VarInteger.Write(ref writer, value);
      Assert.Equal(expectedLength, written);

      // 3. Read
      var sequence = new ReadOnlySequence<byte>(buffer.ToArray());
      var reader = new SequenceReader<byte>(sequence);
      Assert.True(VarInteger.TryRead(ref reader, out int result));
      Assert.Equal(value, result);
   }

   [Fact]
   public void TestInt32RandomValues()
   {
      var rand = new Random(43);
      Span<byte> buffer = stackalloc byte[5];

      for (int i = 0; i < 10000; i++)
      {
         var value = rand.Next(int.MinValue, int.MaxValue);

         var calculatedLength = VarInteger.CalculateByteLength(value);
         var writer = new BufferWriter<byte>(buffer);
         VarInteger.Write(ref writer, value);
         Assert.Equal(calculatedLength, writer.Position);

         var sequence = new ReadOnlySequence<byte>(buffer[..calculatedLength].ToArray());
         var reader = new SequenceReader<byte>(sequence);
         Assert.True(VarInteger.TryRead(ref reader, out int result));
         Assert.Equal(value, result);
      }
   }

   #endregion

   #region Uint64 Tests

   [Theory]
   [InlineData(0ul, 1)]
   [InlineData(1ul, 1)]
   [InlineData(127ul, 1)]
   [InlineData(128ul, 2)]
   [InlineData(16383ul, 2)]
   [InlineData(16384ul, 3)]
   [InlineData(2097151ul, 3)]
   [InlineData(2097152ul, 4)]
   [InlineData(268435455ul, 4)]
   [InlineData(268435456ul, 5)]
   [InlineData(34359738367ul, 5)]
   [InlineData(34359738368ul, 6)]
   [InlineData(4398046511103ul, 6)]
   [InlineData(4398046511104ul, 7)]
   [InlineData(562949953421311ul, 7)]
   [InlineData(562949953421312ul, 8)]
   [InlineData(72057594037927935ul, 8)]
   [InlineData(72057594037927936ul, 9)]
   [InlineData(9223372036854775807ul, 9)]
   [InlineData(9223372036854775808ul, 10)]
   [InlineData(ulong.MaxValue, 10)]
   public void TestUint64RoundTripAndLength(ulong value, int expectedLength)
   {
      // 1. Calculate length
      var calculatedLength = VarInteger.CalculateByteLength(value);
      Assert.Equal(expectedLength, calculatedLength);

      // 2. Write
      Span<byte> buffer = stackalloc byte[calculatedLength];
      var writer = new BufferWriter<byte>(buffer);
      var written = VarInteger.Write(ref writer, value);
      Assert.Equal(expectedLength, written);

      // 3. Read
      var sequence = new ReadOnlySequence<byte>(buffer.ToArray());
      var reader = new SequenceReader<byte>(sequence);
      Assert.True(VarInteger.TryRead(ref reader, out ulong result));
      Assert.Equal(value, result);
   }

   [Fact]
   public void TestUint64RandomValues()
   {
      var rand = new Random(44);
      Span<byte> buffer = stackalloc byte[10];

      for (int i = 0; i < 10000; i++)
      {
         var high = (ulong)rand.Next();
         var low = (ulong)rand.Next();
         var value = (high << 32) | low;

         var calculatedLength = VarInteger.CalculateByteLength(value);
         var writer = new BufferWriter<byte>(buffer);
         VarInteger.Write(ref writer, value);
         Assert.Equal(calculatedLength, writer.Position);

         var sequence = new ReadOnlySequence<byte>(buffer[..calculatedLength].ToArray());
         var reader = new SequenceReader<byte>(sequence);
         Assert.True(VarInteger.TryRead(ref reader, out ulong result));
         Assert.Equal(value, result);
      }
   }

   #endregion

   #region Int64 Tests

    [Theory]
    [InlineData(0L, 1)]
    [InlineData(1L, 1)]
    [InlineData(-1L, 1)]
    [InlineData(63L, 1)]
    [InlineData(-63L, 1)]
    [InlineData(64L, 2)]
    [InlineData(-64L, 1)]
    [InlineData(-65L, 2)]
    [InlineData(8191L, 2)]
    [InlineData(-8191L, 2)]
    [InlineData(8192L, 3)]
    [InlineData(-8192L, 2)]
    [InlineData(-8193L, 3)]
    [InlineData(long.MaxValue, 10)]
    [InlineData(long.MinValue, 10)]
    public void TestInt64RoundTripAndLength(long value, int expectedLength)
   {
      // 1. Calculate length
      var calculatedLength = VarInteger.CalculateByteLength(value);
      Assert.Equal(expectedLength, calculatedLength);

      // 2. Write
      Span<byte> buffer = stackalloc byte[calculatedLength];
      var writer = new BufferWriter<byte>(buffer);
      var written = VarInteger.Write(ref writer, value);
      Assert.Equal(expectedLength, written);

      // 3. Read
      var sequence = new ReadOnlySequence<byte>(buffer.ToArray());
      var reader = new SequenceReader<byte>(sequence);
      Assert.True(VarInteger.TryRead(ref reader, out long result));
      Assert.Equal(value, result);
   }

   [Fact]
   public void TestInt64RandomValues()
   {
      var rand = new Random(45);
      Span<byte> buffer = stackalloc byte[10];

      for (int i = 0; i < 10000; i++)
      {
         var high = (ulong)rand.Next();
         var low = (ulong)rand.Next();
         var value = (long)((high << 32) | low);

         var calculatedLength = VarInteger.CalculateByteLength(value);
         var writer = new BufferWriter<byte>(buffer);
         VarInteger.Write(ref writer, value);
         Assert.Equal(calculatedLength, writer.Position);

         var sequence = new ReadOnlySequence<byte>(buffer[..calculatedLength].ToArray());
         var reader = new SequenceReader<byte>(sequence);
         Assert.True(VarInteger.TryRead(ref reader, out long result));
         Assert.Equal(value, result);
      }
   }

   #endregion

   #region Boundary & Error Cases

   [Fact]
   public void TestTryReadEmptySequenceReturnsFalse()
   {
      var reader = new SequenceReader<byte>(ReadOnlySequence<byte>.Empty);
      Assert.False(VarInteger.TryRead(ref reader, out uint u32));
      Assert.False(VarInteger.TryRead(ref reader, out int s32));
      Assert.False(VarInteger.TryRead(ref reader, out ulong u64));
      Assert.False(VarInteger.TryRead(ref reader, out long s64));
   }

   [Fact]
   public void TestTryReadTruncatedSequenceReturnsFalse()
   {
      // uint 16384 takes 3 bytes: 0x80, 0x80, 0x01
      uint value = 16384u;
      Span<byte> buffer = stackalloc byte[3];
      var writer = new BufferWriter<byte>(buffer);
      VarInteger.Write(ref writer, value);

      // Present only 2 bytes
      var sequence = new ReadOnlySequence<byte>(buffer[..2].ToArray());
      var reader = new SequenceReader<byte>(sequence);

      Assert.False(VarInteger.TryRead(ref reader, out uint result));
   }

   [Fact]
   public void TestInvalidVarintUint32ThrowsFormatException()
   {
      // 6 bytes with MSB set to 1. Since uint is max 5 bytes, this should trigger FormatException
      var invalidBytes = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 };
      var sequence = new ReadOnlySequence<byte>(invalidBytes);
      var reader = new SequenceReader<byte>(sequence);

      var exceptionThrown = false;
      try
      {
         VarInteger.TryRead(ref reader, out uint _);
      }
      catch (FormatException)
      {
         exceptionThrown = true;
      }
      Assert.True(exceptionThrown);
      
      var reader2 = new SequenceReader<byte>(sequence);
      var exceptionThrown2 = false;
      try
      {
         VarInteger.TryRead(ref reader2, out int _);
      }
      catch (FormatException)
      {
         exceptionThrown2 = true;
      }
      Assert.True(exceptionThrown2);
   }

   [Fact]
   public void TestInvalidVarintUint64ThrowsFormatException()
   {
      // 11 bytes with MSB set to 1. Since ulong is max 10 bytes, this should trigger FormatException
      var invalidBytes = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 };
      var sequence = new ReadOnlySequence<byte>(invalidBytes);
      var reader = new SequenceReader<byte>(sequence);

      var exceptionThrown = false;
      try
      {
         VarInteger.TryRead(ref reader, out ulong _);
      }
      catch (FormatException)
      {
         exceptionThrown = true;
      }
      Assert.True(exceptionThrown);
      
      var reader2 = new SequenceReader<byte>(sequence);
      var exceptionThrown2 = false;
      try
      {
         VarInteger.TryRead(ref reader2, out long _);
      }
      catch (FormatException)
      {
         exceptionThrown2 = true;
      }
      Assert.True(exceptionThrown2);
   }

   #endregion
}
