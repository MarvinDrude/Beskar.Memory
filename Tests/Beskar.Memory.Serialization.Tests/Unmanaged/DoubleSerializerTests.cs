using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class DoubleSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<double>.GetWrite();
      var tryRead = SerializerRegistry<double>.GetTryRead();
      var calculate = SerializerRegistry<double>.GetCalculateByteLength();

      var value = 12345.6789;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out double result));
      Assert.Equal(value, result);
   }
}
