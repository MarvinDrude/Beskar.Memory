using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class HashSetSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<HashSet<int>?>.GetWrite();
      var tryRead = SerializerRegistry<HashSet<int>?>.GetTryRead();
      var calculate = SerializerRegistry<HashSet<int>?>.GetCalculateByteLength();

      var value = new HashSet<int> { 10, 20, 30 };
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out HashSet<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Contains(20, result);
   }
}
