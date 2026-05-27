using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class HalfSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Half>.GetWrite();
      var tryRead = SerializerRegistry<Half>.GetTryRead();
      var calculate = SerializerRegistry<Half>.GetCalculateByteLength();

      var value = (Half)12.34f;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Half result));
      Assert.Equal(value, result);
   }
}
