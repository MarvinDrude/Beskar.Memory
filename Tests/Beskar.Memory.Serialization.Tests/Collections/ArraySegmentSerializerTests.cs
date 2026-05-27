using System;
using System.Buffers;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class ArraySegmentSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ArraySegment<int>>.GetWrite();
      var tryRead = SerializerRegistry<ArraySegment<int>>.GetTryRead();
      var calculate = SerializerRegistry<ArraySegment<int>>.GetCalculateByteLength();

      int[] raw = [10, 20, 30, 40, 50];
      var value = new ArraySegment<int>(raw, 1, 3);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ArraySegment<int> result));
      Assert.Equal(value.Count, result.Count);
      Assert.Equal(value[0], result[0]);
      Assert.Equal(value[2], result[2]);
   }
}
