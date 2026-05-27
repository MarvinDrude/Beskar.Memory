using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class DateOnlySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<DateOnly>.GetWrite();
      var tryRead = SerializerRegistry<DateOnly>.GetTryRead();
      var calculate = SerializerRegistry<DateOnly>.GetCalculateByteLength();

      var value = new DateOnly(2026, 5, 27);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out DateOnly result));
      Assert.Equal(value, result);
   }
}
