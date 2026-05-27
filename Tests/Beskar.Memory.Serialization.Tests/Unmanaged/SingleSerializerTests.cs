using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class SingleSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<float>.GetWrite();
      var tryRead = SerializerRegistry<float>.GetTryRead();
      var calculate = SerializerRegistry<float>.GetCalculateByteLength();

      var value = 12345.67f;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out float result));
      Assert.Equal(value, result);
   }
}
