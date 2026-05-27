using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class Int64SerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<long>.GetWrite();
      var tryRead = SerializerRegistry<long>.GetTryRead();
      var calculate = SerializerRegistry<long>.GetCalculateByteLength();

      var value = -123456789012345L;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out long result));
      Assert.Equal(value, result);
   }
}
