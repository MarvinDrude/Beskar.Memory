using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class UInt32SerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<uint>.GetWrite();
      var tryRead = SerializerRegistry<uint>.GetTryRead();
      var calculate = SerializerRegistry<uint>.GetCalculateByteLength();

      var value = 123456789U;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out uint result));
      Assert.Equal(value, result);
   }
}
