using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class UriSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Uri?>.GetWrite();
      var tryRead = SerializerRegistry<Uri?>.GetTryRead();
      var calculate = SerializerRegistry<Uri?>.GetCalculateByteLength();

      var value = new Uri("https://github.com/MarvinDrude/Beskar.Memory");
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Uri? result));
      Assert.NotNull(result);
      Assert.Equal(value, result);
   }
}
