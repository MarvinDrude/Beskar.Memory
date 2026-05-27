using System;
using System.Buffers;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization;
using Beskar.Memory.Serialization.Interfaces;
using Beskar.Memory.Serialization.Resolvers;

namespace Beskar.Memory.Tests.Serialization;

public class SerializerResolverTests
{
   // Custom class to test explicit registration
   public class CustomTestClass
   {
      public string Name { get; set; } = string.Empty;
   }

   public class CustomTestClassSerializer : ISerializer<CustomTestClass?>
   {
      public static int Write(ref BufferWriter<byte> writer, scoped in CustomTestClass? value)
      {
         if (value is null)
         {
            var lengthSpan = writer.AcquireSpan(sizeof(int));
            System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(lengthSpan, -1);
            return sizeof(int);
         }

         var bytes = System.Text.Encoding.UTF8.GetBytes(value.Name);
         var lenSpan = writer.AcquireSpan(sizeof(int));
         System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(lenSpan, bytes.Length);
         var contentSpan = writer.AcquireSpan(bytes.Length);
         bytes.CopyTo(contentSpan);
         return sizeof(int) + bytes.Length;
      }

      public static bool TryRead(ref SequenceReader<byte> reader, out CustomTestClass? value)
      {
         if (!reader.TryReadLittleEndian(out int length))
         {
            value = null;
            return false;
         }
         if (length < 0)
         {
            value = null;
            return true;
         }
         var buffer = new byte[length];
         for (var i = 0; i < length; i++)
         {
            if (!reader.TryRead(out var b))
            {
               value = null;
               return false;
            }
            buffer[i] = b;
         }
         value = new CustomTestClass { Name = System.Text.Encoding.UTF8.GetString(buffer) };
         return true;
      }

      public static int CalculateByteLength(scoped in CustomTestClass? value)
      {
         if (value is null) return sizeof(int);
         return sizeof(int) + System.Text.Encoding.UTF8.GetByteCount(value.Name);
      }
   }

   [Fact]
   public void TestListOnDemandResolution()
   {
      // Act: Get delegates for a type that was never registered in DefaultSerializerRegistration
      var write = SerializerRegistry<List<int>?>.Write;
      var tryRead = SerializerRegistry<List<int>?>.TryRead;
      var calc = SerializerRegistry<List<int>?>.CalculateByteLength;

      // Assert: They should be automatically resolved and non-null
      Assert.NotNull(write);
      Assert.NotNull(tryRead);
      Assert.NotNull(calc);

      // Verify functionality
      var list = new List<int> { 10, 20, 30 };
      var length = calc(list);

      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      var written = write(ref writer, list);

      Assert.Equal(length, written);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      var readSuccess = tryRead(ref reader, out var result);

      Assert.True(readSuccess);
      Assert.NotNull(result);
      Assert.Equal(3, result.Count);
      Assert.Equal(10, result[0]);
      Assert.Equal(30, result[2]);
   }

   [Fact]
   public void TestArrayOnDemandResolution()
   {
      var write = SerializerRegistry<string[]?>.Write;
      var tryRead = SerializerRegistry<string[]?>.TryRead;

      Assert.NotNull(write);
      Assert.NotNull(tryRead);

      var arr = new string[] { "hello", "world" };
      var length = SerializerRegistry<string[]?>.GetCalculateByteLength()(arr);

      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, arr);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      var success = tryRead(ref reader, out var result);

      Assert.True(success);
      Assert.NotNull(result);
      Assert.Equal(2, result.Length);
      Assert.Equal("hello", result[0]);
      Assert.Equal("world", result[1]);
   }

   [Fact]
   public void TestExplicitMappingResolution()
   {
      // Register custom mapping in non-generic SerializerRegistry
      SerializerRegistry.Register(typeof(CustomTestClass), typeof(CustomTestClassSerializer));

      // Act: retrieve from generic SerializerRegistry
      var write = SerializerRegistry<CustomTestClass?>.Write;
      Assert.NotNull(write);

      var value = new CustomTestClass { Name = "Antigravity" };
      var length = SerializerRegistry<CustomTestClass?>.GetCalculateByteLength()(value);

      var buffer = new byte[length];
      var writer = new BufferWriter<byte>(buffer);
      write(ref writer, value);

      var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
      var success = SerializerRegistry<CustomTestClass?>.GetTryRead()(ref reader, out var result);

      Assert.True(success);
      Assert.NotNull(result);
      Assert.Equal("Antigravity", result.Name);
   }
}
