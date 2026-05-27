using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public enum TestEnum
{
   FirstValue = 10,
   SecondValue = 20,
   ThirdValue = 30
}

public class EnumSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<TestEnum>.GetWrite();
      var tryRead = SerializerRegistry<TestEnum>.GetTryRead();
      var calculate = SerializerRegistry<TestEnum>.GetCalculateByteLength();

      var value = TestEnum.SecondValue;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out TestEnum result));
      Assert.Equal(value, result);
   }
}
