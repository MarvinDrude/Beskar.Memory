using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Immutable;

public class ImmutableDictionarySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ImmutableDictionary<int, string>?>.GetWrite();
      var tryRead = SerializerRegistry<ImmutableDictionary<int, string>?>.GetTryRead();
      var calculate = SerializerRegistry<ImmutableDictionary<int, string>?>.GetCalculateByteLength();

      var value = ImmutableDictionary.CreateRange(new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } });
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ImmutableDictionary<int, string>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal("One", result[1]);
   }
}
