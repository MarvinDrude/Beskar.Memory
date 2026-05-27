using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class TimeOnlySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<TimeOnly>.GetWrite();
      var tryRead = SerializerRegistry<TimeOnly>.GetTryRead();
      var calculate = SerializerRegistry<TimeOnly>.GetCalculateByteLength();

      var value = new TimeOnly(14, 35, 10);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out TimeOnly result));
      Assert.Equal(value, result);
   }
}
