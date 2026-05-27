using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class DecimalSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<decimal>.GetWrite();
      var tryRead = SerializerRegistry<decimal>.GetTryRead();
      var calculate = SerializerRegistry<decimal>.GetCalculateByteLength();

      var value = 12345.6789m;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out decimal result));
      Assert.Equal(value, result);
   }
}
