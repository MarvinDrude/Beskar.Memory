using System;
using System.Buffers;
using System.Collections.Immutable;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Immutable;

public class ImmutableHashSetSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ImmutableHashSet<int>?>.GetWrite();
      var tryRead = SerializerRegistry<ImmutableHashSet<int>?>.GetTryRead();
      var calculate = SerializerRegistry<ImmutableHashSet<int>?>.GetCalculateByteLength();

      var value = ImmutableHashSet.Create(10, 20, 30);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ImmutableHashSet<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Contains(20, result);
   }
}
