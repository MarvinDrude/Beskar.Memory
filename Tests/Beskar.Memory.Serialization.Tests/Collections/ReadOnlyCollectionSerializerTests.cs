using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections;

public class ReadOnlyCollectionSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ReadOnlyCollection<int>?>.GetWrite();
      var tryRead = SerializerRegistry<ReadOnlyCollection<int>?>.GetTryRead();
      var calculate = SerializerRegistry<ReadOnlyCollection<int>?>.GetCalculateByteLength();

      var value = new ReadOnlyCollection<int>(new List<int> { 10, 20, 30 });
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ReadOnlyCollection<int>? result));
      Assert.NotNull(result);
      Assert.Equal(value.Count, result.Count);
      Assert.Equal(20, result[1]);
   }
}
