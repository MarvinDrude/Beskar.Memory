using System;
using System.Buffers;
using Beskar.Memory.Writers;
using Xunit;

namespace Beskar.Memory.Serialization.Tests;

public class BeSerializerTests
{
   private const int TestValue = 1337;

   [Fact]
   public void TestSerializeToByteArray()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      Assert.NotNull(bytes);
      Assert.Equal(2, bytes.Length);

      var deserialized = BeSerializer.Deserialize<int>(bytes);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestSerializeToSpan()
   {
      Span<byte> buffer = stackalloc byte[16];
      var written = BeSerializer.Serialize(TestValue, buffer);
      Assert.Equal(2, written);

      var deserialized = BeSerializer.Deserialize<int>(buffer[..written]);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestSerializeToBufferWriter()
   {
      Span<byte> buffer = stackalloc byte[16];
      var writer = new BufferWriter<byte>(buffer);
      try
      {
         var written = BeSerializer.Serialize(TestValue, ref writer);
         Assert.Equal(2, written);
         Assert.Equal(2, writer.Position);

         var deserialized = BeSerializer.Deserialize<int>(writer.WrittenSpan);
         Assert.Equal(TestValue, deserialized);
      }
      finally
      {
         writer.Dispose();
      }
   }

   [Fact]
   public void TestCalculateByteLength()
   {
      var length = BeSerializer.CalculateByteLength(TestValue);
      Assert.Equal(2, length);
   }

   [Fact]
   public void TestDeserializeFromReadOnlySequence()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);

      var deserialized = BeSerializer.Deserialize<int>(sequence);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromReadOnlyMemory()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var memory = new ReadOnlyMemory<byte>(bytes);

      var deserialized = BeSerializer.Deserialize<int>(memory);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromByteArray()
   {
      var bytes = BeSerializer.Serialize(TestValue);

      var deserialized = BeSerializer.Deserialize<int>(bytes);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromReadOnlySpan()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      ReadOnlySpan<byte> span = bytes.AsSpan();

      var deserialized = BeSerializer.Deserialize<int>(span);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromSequenceReader()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var reader = new SequenceReader<byte>(sequence);

      var deserialized = BeSerializer.Deserialize<int>(ref reader);
      Assert.Equal(TestValue, deserialized);
      Assert.Equal(2, reader.Consumed);
   }

   [Fact]
   public void TestTryDeserializeFromReadOnlySequence()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);

      var success = BeSerializer.TryDeserialize<int>(sequence, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromReadOnlyMemory()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var memory = new ReadOnlyMemory<byte>(bytes);

      var success = BeSerializer.TryDeserialize<int>(memory, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromByteArray()
   {
      var bytes = BeSerializer.Serialize(TestValue);

      var success = BeSerializer.TryDeserialize<int>(bytes, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromReadOnlySpan()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      ReadOnlySpan<byte> span = bytes.AsSpan();

      var success = BeSerializer.TryDeserialize<int>(span, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromSequenceReader()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var reader = new SequenceReader<byte>(sequence);

      var success = BeSerializer.TryDeserialize<int>(ref reader, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
      Assert.Equal(2, reader.Consumed);
   }
}
