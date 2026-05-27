using System;
using System.Buffers;
using System.Collections.Concurrent;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Concurrent;

public class ConcurrentDictionarySerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ConcurrentDictionary<int, string>?>.GetWrite();
      var tryRead = SerializerRegistry<ConcurrentDictionary<int, string>?>.GetTryRead();
      var calculate = SerializerRegistry<ConcurrentDictionary<int, string>?>.GetCalculateByteLength();

      var value = new ConcurrentDictionary<int, string>();
      value.TryAdd(1, "One");
      value.TryAdd(2, "Two");
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ConcurrentDictionary<int, string>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal("One", result[1]);
   }
}
