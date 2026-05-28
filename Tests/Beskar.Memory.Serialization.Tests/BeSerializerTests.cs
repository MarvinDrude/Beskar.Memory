using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;
using Beskar.Memory.Serialization;
using Beskar.Memory.Serialization.Serializers;
using Xunit;

namespace Beskar.Memory.Serialization.Tests;

public class BeSerializerTests
{
   private const int TestValue = 1337;

   [Fact]
   public void TestSerializeToByteArray()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      Assert.NotNull(bytes);
      Assert.Equal(2, bytes.Length);

      var deserialized = BeSerializer.Deserialize<int>(bytes);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestSerializeToSpan()
   {
      Span<byte> buffer = stackalloc byte[16];
      var written = BeSerializer.Serialize(TestValue, buffer);
      Assert.Equal(2, written);

      var deserialized = BeSerializer.Deserialize<int>(buffer[..written]);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestSerializeToBufferWriter()
   {
      Span<byte> buffer = stackalloc byte[16];
      var writer = new BufferWriter<byte>(buffer);
      try
      {
         var written = BeSerializer.Serialize(TestValue, ref writer);
         Assert.Equal(2, written);
         Assert.Equal(2, writer.Position);

         var deserialized = BeSerializer.Deserialize<int>(writer.WrittenSpan);
         Assert.Equal(TestValue, deserialized);
      }
      finally
      {
         writer.Dispose();
      }
   }

   [Fact]
   public void TestCalculateByteLength()
   {
      var length = BeSerializer.CalculateByteLength(TestValue);
      Assert.Equal(2, length);
   }

   [Fact]
   public void TestDeserializeFromReadOnlySequence()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);

      var deserialized = BeSerializer.Deserialize<int>(sequence);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromReadOnlyMemory()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var memory = new ReadOnlyMemory<byte>(bytes);

      var deserialized = BeSerializer.Deserialize<int>(memory);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromByteArray()
   {
      var bytes = BeSerializer.Serialize(TestValue);

      var deserialized = BeSerializer.Deserialize<int>(bytes);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromReadOnlySpan()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      ReadOnlySpan<byte> span = bytes.AsSpan();

      var deserialized = BeSerializer.Deserialize<int>(span);
      Assert.Equal(TestValue, deserialized);
   }

   [Fact]
   public void TestDeserializeFromSequenceReader()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var reader = new SequenceReader<byte>(sequence);

      var deserialized = BeSerializer.Deserialize<int>(ref reader);
      Assert.Equal(TestValue, deserialized);
      Assert.Equal(2, reader.Consumed);
   }

   [Fact]
   public void TestTryDeserializeFromReadOnlySequence()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);

      var success = BeSerializer.TryDeserialize<int>(sequence, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromReadOnlyMemory()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var memory = new ReadOnlyMemory<byte>(bytes);

      var success = BeSerializer.TryDeserialize<int>(memory, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromByteArray()
   {
      var bytes = BeSerializer.Serialize(TestValue);

      var success = BeSerializer.TryDeserialize<int>(bytes, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromReadOnlySpan()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      ReadOnlySpan<byte> span = bytes.AsSpan();

      var success = BeSerializer.TryDeserialize<int>(span, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
   }

   [Fact]
   public void TestTryDeserializeFromSequenceReader()
   {
      var bytes = BeSerializer.Serialize(TestValue);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var reader = new SequenceReader<byte>(sequence);

      var success = BeSerializer.TryDeserialize<int>(ref reader, out int value);
      Assert.True(success);
      Assert.Equal(TestValue, value);
      Assert.Equal(2, reader.Consumed);
   }

   [Fact]
   public void TestCyclicReference()
   {
      SerializerRegistry<CyclicNode>.Register<CyclicNodeSerializer>();
      SerializerRegistry<CyclicNode?>.Register<CyclicNodeSerializer>();

      var node1 = new CyclicNode { Name = "Node 1" };
      var node2 = new CyclicNode { Name = "Node 2" };
      node1.Next = node2;
      node2.Next = node1; // Cycle!

      var bytes = BeSerializer.Serialize(node1);
      Assert.NotNull(bytes);

      var deserialized = BeSerializer.Deserialize<CyclicNode>(bytes);
      Assert.NotNull(deserialized);
      Assert.Equal("Node 1", deserialized.Name);
      Assert.NotNull(deserialized.Next);
      Assert.Equal("Node 2", deserialized.Next.Name);
      Assert.Same(deserialized, deserialized.Next.Next); // Cycle is preserved!
   }

   [Fact]
   public void TestLargeCyclicReference()
   {
      SerializerRegistry<CyclicNode>.Register<CyclicNodeSerializer>();
      SerializerRegistry<CyclicNode?>.Register<CyclicNodeSerializer>();

      // Create a cyclic chain of 20 nodes to exceed the 16-element limit and trigger dictionary fallback
      var count = 20;
      var nodes = new CyclicNode[count];
      for (var i = 0; i < count; i++)
      {
         nodes[i] = new CyclicNode { Name = $"Node {i}" };
      }

      for (var i = 0; i < count; i++)
      {
         nodes[i].Next = nodes[(i + 1) % count]; // Cycle!
      }

      var bytes = BeSerializer.Serialize(nodes[0]);
      Assert.NotNull(bytes);

      var deserialized = BeSerializer.Deserialize<CyclicNode>(bytes);
      Assert.NotNull(deserialized);

      // Verify the full cycle is preserved correctly via reference equality in dictionary fallback
      var current = deserialized;
      var seen = new global::System.Collections.Generic.HashSet<CyclicNode>();
      for (var i = 0; i < count; i++)
      {
         Assert.NotNull(current);
         Assert.Equal($"Node {i}", current.Name);
         Assert.True(seen.Add(current));
         current = current.Next;
      }
      Assert.Same(deserialized, current); // Cycles back to start!
   }
}

public class CyclicNode
{
   public required string Name { get; set; }
   public CyclicNode? Next { get; set; }
}

public sealed class CyclicNodeSerializer : ISerializer<CyclicNode>, ISerializer<CyclicNode?>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in CyclicNode? value)
   {
      var bytesWritten = 0;
      if (value is null)
      {
         return VarInteger.Write(ref writer, 0);
      }

      ref var context = ref SerializationContext.Current;
      if (context.TryGetReferenceId(value, out int refId))
      {
         return VarInteger.Write(ref writer, -refId);
      }

      int newId = context.Register(value);
      bytesWritten = VarInteger.Write(ref writer, newId);

      bytesWritten += SerializerRegistry<string>.GetWrite()(ref writer, value.Name);
      bytesWritten += SerializerRegistry<CyclicNode?>.GetWrite()(ref writer, value.Next);

      return bytesWritten;
   }

   public static bool TryRead(ref SequenceReader<byte> reader, [MaybeNullWhen(false)] out CyclicNode? value)
   {
      if (!VarInteger.TryRead(ref reader, out int refTag))
      {
         value = default;
         return false;
      }

      if (refTag == 0)
      {
         value = default;
         return true;
      }

      ref var context = ref DeserializationContext.Current;
      if (refTag < 0)
      {
         value = (CyclicNode)context.GetByReferenceId(-refTag);
         return true;
      }

      value = new CyclicNode { Name = null! };
      context.Register(refTag, value);

      if (!SerializerRegistry<string>.GetTryRead()(ref reader, out var member_Name))
      {
         value = default;
         return false;
      }
      value.Name = member_Name;

      if (!SerializerRegistry<CyclicNode?>.GetTryRead()(ref reader, out var member_Next))
      {
         value = default;
         return false;
      }
      value.Next = member_Next;

      return true;
   }

   public static int CalculateByteLength(scoped in CyclicNode? value)
   {
      var totalLength = 0;
      if (value is null)
      {
         return VarInteger.CalculateByteLength(0);
      }

      ref var context = ref SerializationContext.Current;
      if (context.TryGetReferenceId(value, out int refId))
      {
         return VarInteger.CalculateByteLength(-refId);
      }

      int newId = context.Register(value);
      totalLength = VarInteger.CalculateByteLength(newId);

      totalLength += SerializerRegistry<string>.GetCalculateByteLength()(value.Name);
      totalLength += SerializerRegistry<CyclicNode?>.GetCalculateByteLength()(value.Next);

      return totalLength;
   }
}
