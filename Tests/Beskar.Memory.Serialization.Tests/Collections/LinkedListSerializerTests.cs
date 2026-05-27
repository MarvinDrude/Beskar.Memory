using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class LinkedListSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<LinkedList<int>?>.GetWrite();
      var tryRead = SerializerRegistry<LinkedList<int>?>.GetTryRead();
      var calculate = SerializerRegistry<LinkedList<int>?>.GetCalculateByteLength();

      var value = new LinkedList<int>();
      value.AddLast(10);
      value.AddLast(20);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out LinkedList<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal(10, result.First!.Value);
   }
}
