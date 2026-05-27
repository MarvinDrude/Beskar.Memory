using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Unmanaged;

public class CharSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<char>.GetWrite();
      var tryRead = SerializerRegistry<char>.GetTryRead();
      var calculate = SerializerRegistry<char>.GetCalculateByteLength();

      var value = 'X';
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out char result));
      Assert.Equal(value, result);
   }
}
