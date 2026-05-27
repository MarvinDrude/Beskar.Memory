using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class DateTimeSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<DateTime>.GetWrite();
      var tryRead = SerializerRegistry<DateTime>.GetTryRead();
      var calculate = SerializerRegistry<DateTime>.GetCalculateByteLength();

      var now = DateTime.UtcNow;
      var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out DateTime result));
      Assert.Equal(value, result);
   }
}
