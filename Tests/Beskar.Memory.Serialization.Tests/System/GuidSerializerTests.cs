using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class GuidSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Guid>.GetWrite();
      var tryRead = SerializerRegistry<Guid>.GetTryRead();
      var calculate = SerializerRegistry<Guid>.GetCalculateByteLength();

      var value = Guid.NewGuid();
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Guid result));
      Assert.Equal(value, result);
   }
}
