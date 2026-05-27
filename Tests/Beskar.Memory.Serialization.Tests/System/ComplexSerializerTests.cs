using System;
using System.Buffers;
using System.Numerics;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class ComplexSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Complex>.GetWrite();
      var tryRead = SerializerRegistry<Complex>.GetTryRead();
      var calculate = SerializerRegistry<Complex>.GetCalculateByteLength();

      var value = new Complex(12.34, -56.78);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Complex result));
      Assert.Equal(value, result);
   }
}
