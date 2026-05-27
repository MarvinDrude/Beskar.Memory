using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class NullableSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<int?>.GetWrite();
      var tryRead = SerializerRegistry<int?>.GetTryRead();
      var calculate = SerializerRegistry<int?>.GetCalculateByteLength();

      int? value = 42;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out int? result));
      Assert.Equal(value, result);
   }
}
