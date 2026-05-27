using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class SortedListSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<SortedList<int, string>?>.GetWrite();
      var tryRead = SerializerRegistry<SortedList<int, string>?>.GetTryRead();
      var calculate = SerializerRegistry<SortedList<int, string>?>.GetCalculateByteLength();

      var value = new SortedList<int, string> { { 2, "Two" }, { 1, "One" } };
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out SortedList<int, string>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal("One", result[1]);
   }
}
