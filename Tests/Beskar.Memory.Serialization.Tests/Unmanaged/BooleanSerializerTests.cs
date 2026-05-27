using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class BooleanSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<bool>.GetWrite();
      var tryRead = SerializerRegistry<bool>.GetTryRead();
      var calculate = SerializerRegistry<bool>.GetCalculateByteLength();

      var value = true;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out bool result));
      Assert.Equal(value, result);
   }
}
