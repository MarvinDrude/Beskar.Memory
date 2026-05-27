using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class ReadOnlyDictionarySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ReadOnlyDictionary<int, string>?>.GetWrite();
      var tryRead = SerializerRegistry<ReadOnlyDictionary<int, string>?>.GetTryRead();
      var calculate = SerializerRegistry<ReadOnlyDictionary<int, string>?>.GetCalculateByteLength();

      var value = new ReadOnlyDictionary<int, string>(new Dictionary<int, string> { { 1, "One" } });
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ReadOnlyDictionary<int, string>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal("One", result[1]);
   }
}
