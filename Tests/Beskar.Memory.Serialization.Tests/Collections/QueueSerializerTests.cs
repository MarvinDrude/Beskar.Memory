using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class QueueSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Queue<int>?>.GetWrite();
      var tryRead = SerializerRegistry<Queue<int>?>.GetTryRead();
      var calculate = SerializerRegistry<Queue<int>?>.GetCalculateByteLength();

      var value = new Queue<int>();
      value.Enqueue(10);
      value.Enqueue(20);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Queue<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal(10, result.Dequeue());
   }
}
