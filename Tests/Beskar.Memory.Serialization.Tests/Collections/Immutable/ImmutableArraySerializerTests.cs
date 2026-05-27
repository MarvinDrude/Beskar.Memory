using System;
using System.Buffers;
using System.Collections.Immutable;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Immutable;

public class ImmutableArraySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ImmutableArray<int>>.GetWrite();
      var tryRead = SerializerRegistry<ImmutableArray<int>>.GetTryRead();
      var calculate = SerializerRegistry<ImmutableArray<int>>.GetCalculateByteLength();

      var value = ImmutableArray.Create(10, 20, 30);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ImmutableArray<int> result));
      Assert.False(result.IsDefault);
      Assert.Equal(value.Length, result.Length);
      Assert.Equal(20, result[1]);
   }
}
