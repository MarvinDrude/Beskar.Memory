using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class SortedDictionarySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<SortedDictionary<int, string>?>.GetWrite();
      var tryRead = SerializerRegistry<SortedDictionary<int, string>?>.GetTryRead();
      var calculate = SerializerRegistry<SortedDictionary<int, string>?>.GetCalculateByteLength();

      var value = new SortedDictionary<int, string> { { 2, "Two" }, { 1, "One" } };
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out SortedDictionary<int, string>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal("One", result[1]);
   }
}
