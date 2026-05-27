using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class UInt64SerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ulong>.GetWrite();
      var tryRead = SerializerRegistry<ulong>.GetTryRead();
      var calculate = SerializerRegistry<ulong>.GetCalculateByteLength();

      var value = 123456789012345UL;
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ulong result));
      Assert.Equal(value, result);
   }
}
