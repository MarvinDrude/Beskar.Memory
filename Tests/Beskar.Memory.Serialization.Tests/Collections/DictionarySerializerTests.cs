using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class DictionarySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<Dictionary<int, string>?>.GetWrite();
      var tryRead = SerializerRegistry<Dictionary<int, string>?>.GetTryRead();
      var calculate = SerializerRegistry<Dictionary<int, string>?>.GetCalculateByteLength();

      var value = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out Dictionary<int, string>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal(value[1], result[1]);
   }
}
