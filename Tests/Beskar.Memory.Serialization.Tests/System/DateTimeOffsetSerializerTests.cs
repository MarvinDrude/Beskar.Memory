using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class DateTimeOffsetSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<DateTimeOffset>.GetWrite();
      var tryRead = SerializerRegistry<DateTimeOffset>.GetTryRead();
      var calculate = SerializerRegistry<DateTimeOffset>.GetCalculateByteLength();

      var now = DateTimeOffset.UtcNow;
      var value = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Offset);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out DateTimeOffset result));
      Assert.Equal(value, result);
   }
}
