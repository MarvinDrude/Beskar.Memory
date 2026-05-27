using System;
using System.Buffers;
using System.Collections.Concurrent;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Concurrent;

public class ConcurrentStackSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ConcurrentStack<int>?>.GetWrite();
      var tryRead = SerializerRegistry<ConcurrentStack<int>?>.GetTryRead();
      var calculate = SerializerRegistry<ConcurrentStack<int>?>.GetCalculateByteLength();

      var value = new ConcurrentStack<int>();
      value.Push(10);
      value.Push(20);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ConcurrentStack<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.True(result.TryPop(out var top));
      Assert.Equal(20, top);
   }
}
