using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;
using Beskar.Memory.Extensions;

namespace Beskar.Memory.Tests.Extensions;

public class SpanByteReadExtensionsTests
{
   [Fact]
   public void ReadLittleEndianCorrectValues()
   {
      var buffer = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
      
      var value16 = buffer.AsSpan().ReadLittleEndian<short>(out var read16);
      Assert.Equal(2, read16);
      if (BitConverter.IsLittleEndian)
      {
         Assert.Equal(0x0201, value16);
      }
      else
      {
         Assert.Equal(0x0102, value16);
      }

      var value32 = buffer.AsSpan().ReadLittleEndian<int>(out var read32);
      Assert.Equal(4, read32);
      if (BitConverter.IsLittleEndian)
      {
         Assert.Equal(0x04030201, value32);
      }
      else
      {
         Assert.Equal(0x01020304, value32);
      }

      var value64 = buffer.AsSpan().ReadLittleEndian<long>(out var read64);
      Assert.Equal(8, read64);
      if (BitConverter.IsLittleEndian)
      {
         Assert.Equal(0x0807060504030201, value64);
      }
      else
      {
         Assert.Equal(0x0102030405060708, value64);
      }
   }

   [Fact]
   public void ReadBigEndianCorrectValues()
   {
      var buffer = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
      
      var value16 = buffer.AsSpan().ReadBigEndian<short>(out var read16);
      Assert.Equal(2, read16);
      if (BitConverter.IsLittleEndian)
      {
         Assert.Equal(0x0102, value16);
      }
      else
      {
         Assert.Equal(0x0201, value16);
      }

      var value32 = buffer.AsSpan().ReadBigEndian<int>(out var read32);
      Assert.Equal(4, read32);
      if (BitConverter.IsLittleEndian)
      {
         Assert.Equal(0x01020304, value32);
      }
      else
      {
         Assert.Equal(0x04030201, value32);
      }

      var value64 = buffer.AsSpan().ReadBigEndian<long>(out var read64);
      Assert.Equal(8, read64);
      if (BitConverter.IsLittleEndian)
      {
         Assert.Equal(0x0102030405060708, value64);
      }
      else
      {
         Assert.Equal(0x0807060504030201, value64);
      }
   }

   [Fact]
   public void ReadThrowsWhenBufferTooSmall()
   {
      var buffer = new byte[] { 0x01, 0x02 };
      
      Assert.Throws<ArgumentOutOfRangeException>(() => buffer.AsSpan().ReadLittleEndian<int>(out _));
      Assert.Throws<ArgumentOutOfRangeException>(() => buffer.AsSpan().ReadBigEndian<int>(out _));
   }

   [Fact]
   public void ReadThrowsWhenTypeTooLargeForStackAlloc()
   {
      var buffer = new byte[300];
      
      Assert.Throws<ArgumentOutOfRangeException>(() => buffer.AsSpan().ReadLittleEndian<LargeUnmanagedType>(out _));
      Assert.Throws<ArgumentOutOfRangeException>(() => buffer.AsSpan().ReadBigEndian<LargeUnmanagedType>(out _));
   }

   [InlineArray(300)]
   private struct LargeUnmanagedType
   {
      private byte _element;
   }
}
