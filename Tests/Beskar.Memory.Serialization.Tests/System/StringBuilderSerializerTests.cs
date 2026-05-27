using System;
using System.Buffers;
using System.Text;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.System;

public class StringBuilderSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<StringBuilder?>.GetWrite();
      var tryRead = SerializerRegistry<StringBuilder?>.GetTryRead();
      var calculate = SerializerRegistry<StringBuilder?>.GetCalculateByteLength();

      var value = new StringBuilder("Antigravity C# Builder");
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out StringBuilder? result));
      Assert.NotNull(result);
      Assert.Equal(value.ToString(), result.ToString());
   }
}
