using System;
using System.Buffers;
using System.Numerics;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class BigIntegerSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<BigInteger>.GetWrite();
      var tryRead = SerializerRegistry<BigInteger>.GetTryRead();
      var calculate = SerializerRegistry<BigInteger>.GetCalculateByteLength();

      var value = BigInteger.Parse("987654321098765432109876543210");
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out BigInteger result));
      Assert.Equal(value, result);
   }
}
