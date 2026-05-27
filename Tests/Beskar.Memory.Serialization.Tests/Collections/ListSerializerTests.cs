using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class ListSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<List<int>?>.GetWrite();
      var tryRead = SerializerRegistry<List<int>?>.GetTryRead();
      var calculate = SerializerRegistry<List<int>?>.GetCalculateByteLength();

      var value = new List<int> { 10, 20, 30 };
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out List<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal(20, result[1]);
   }
}
