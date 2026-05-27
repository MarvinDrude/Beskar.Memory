using System;
using System.Buffers;
using System.Collections.Concurrent;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Concurrent;

public class ConcurrentQueueSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ConcurrentQueue<int>?>.GetWrite();
      var tryRead = SerializerRegistry<ConcurrentQueue<int>?>.GetTryRead();
      var calculate = SerializerRegistry<ConcurrentQueue<int>?>.GetCalculateByteLength();

      var value = new ConcurrentQueue<int>();
      value.Enqueue(10);
      value.Enqueue(20);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ConcurrentQueue<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.True(result.TryDequeue(out var first));
      Assert.Equal(10, first);
   }
}
