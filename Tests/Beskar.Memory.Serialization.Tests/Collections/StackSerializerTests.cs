using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class StackSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Stack<int>?>.GetWrite();
      var tryRead = SerializerRegistry<Stack<int>?>.GetTryRead();
      var calculate = SerializerRegistry<Stack<int>?>.GetCalculateByteLength();

      var value = new Stack<int>();
      value.Push(10);
      value.Push(20);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Stack<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal(20, result.Pop());
   }
}
