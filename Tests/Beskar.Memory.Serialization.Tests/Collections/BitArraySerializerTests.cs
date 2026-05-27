using System;
using System.Buffers;
using System.Collections;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class BitArraySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<BitArray?>.GetWrite();
      var tryRead = SerializerRegistry<BitArray?>.GetTryRead();
      var calculate = SerializerRegistry<BitArray?>.GetCalculateByteLength();

      var value = new BitArray(new bool[] { true, false, true, true });
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out BitArray? result));
      Assert.NotNull(result);
      Assert.Equal(value.Length, result.Length);
      Assert.Equal(value[0], result[0]);
      Assert.Equal(value[1], result[1]);
   }
}
