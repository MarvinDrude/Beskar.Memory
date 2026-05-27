using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class ByteSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<byte>.GetWrite();
      var tryRead = SerializerRegistry<byte>.GetTryRead();
      var calculate = SerializerRegistry<byte>.GetCalculateByteLength();

      var value = (byte)123;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out byte result));
      Assert.Equal(value, result);
   }
}
