using System;
using System.Buffers;
using System.Collections.Immutable;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;

namespace Beskar.Memory.Serialization.Tests.Collections.Immutable;

public class ImmutableStackSerializerTests
{
   [Fact]
   public void TestSerialization()
   {
      var write = SerializerRegistry<ImmutableStack<int>?>.GetWrite();
      var tryRead = SerializerRegistry<ImmutableStack<int>?>.GetTryRead();
      var calculate = SerializerRegistry<ImmutableStack<int>?>.GetCalculateByteLength();

      var value = ImmutableStack.Create(10, 20);
      var length = calculate(value);
      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      Assert.True(tryRead(ref reader, out ImmutableStack<int>? result));
      Assert.NotNull(result);
      Assert.False(result.IsEmpty);
      var stack = result.Pop(out var top);
      Assert.Equal(20, top);
   }
}
