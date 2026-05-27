using System;
using System.Buffers;
using System.Collections.Immutable;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Immutable;

public class ImmutableSortedSetSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ImmutableSortedSet<int>?>.GetWrite();
      var tryRead = SerializerRegistry<ImmutableSortedSet<int>?>.GetTryRead();
      var calculate = SerializerRegistry<ImmutableSortedSet<int>?>.GetCalculateByteLength();

      var value = ImmutableSortedSet.Create(10, 20, 30);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ImmutableSortedSet<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Contains(20, result);
   }
}
