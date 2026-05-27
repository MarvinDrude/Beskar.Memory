using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class StringSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<string?>.GetWrite();
      var tryRead = SerializerRegistry<string?>.GetTryRead();
      var calculate = SerializerRegistry<string?>.GetCalculateByteLength();

      var value = "Dynamic Serialization System";
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out string? result));
      Assert.Equal(value, result);
   }
}
