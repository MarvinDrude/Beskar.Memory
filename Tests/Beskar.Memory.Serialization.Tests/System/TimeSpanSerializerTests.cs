using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class TimeSpanSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<TimeSpan>.GetWrite();
      var tryRead = SerializerRegistry<TimeSpan>.GetTryRead();
      var calculate = SerializerRegistry<TimeSpan>.GetCalculateByteLength();

      var value = TimeSpan.FromHours(5.5);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out TimeSpan result));
      Assert.Equal(value, result);
   }
}
