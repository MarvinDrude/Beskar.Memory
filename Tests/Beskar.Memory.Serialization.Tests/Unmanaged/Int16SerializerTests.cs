using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class Int16SerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<short>.GetWrite();
      var tryRead = SerializerRegistry<short>.GetTryRead();
      var calculate = SerializerRegistry<short>.GetCalculateByteLength();

      var value = (short)-12345;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out short result));
      Assert.Equal(value, result);
   }
}
