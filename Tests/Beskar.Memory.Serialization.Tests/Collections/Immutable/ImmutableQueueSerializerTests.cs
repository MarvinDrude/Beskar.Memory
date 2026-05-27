using System;
using System.Buffers;
using System.Collections.Immutable;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Immutable;

public class ImmutableQueueSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ImmutableQueue<int>?>.GetWrite();
      var tryRead = SerializerRegistry<ImmutableQueue<int>?>.GetTryRead();
      var calculate = SerializerRegistry<ImmutableQueue<int>?>.GetCalculateByteLength();

      var value = ImmutableQueue.Create(10, 20);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ImmutableQueue<int>? result));
      Assert.NotNull(result);
      Assert.False(result.IsEmpty);
      var queue = result.Dequeue(out var first);
      Assert.Equal(10, first);
   }
}
