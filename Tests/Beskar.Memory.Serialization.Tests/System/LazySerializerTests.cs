using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class LazySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Lazy<int>?>.GetWrite();
      var tryRead = SerializerRegistry<Lazy<int>?>.GetTryRead();
      var calculate = SerializerRegistry<Lazy<int>?>.GetCalculateByteLength();

      var value = new Lazy<int>(() => 42);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Lazy<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Value, result.Value);
   }
}
