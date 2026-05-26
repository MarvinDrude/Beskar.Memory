using System;
using Xunit;
using Beskar.Memory.Flags;

namespace Beskar.Memory.Tests.Flags;

public class FlagsTests
{
   [Fact]
   public void PackedBools8Operations()
   {
      var p = new PackedBools8(0);
      
      Assert.Equal(0, p.RawByte);
      Assert.Equal(0, p.CountSetBits());
      
      p.Set(0, true);
      p.Set(7, true);
      
      Assert.True(p.Get(0));
      Assert.True(p[7]);
      Assert.False(p[3]);
      Assert.Equal(2, p.CountSetBits());
      
      p[7] = false;
      Assert.False(p[7]);
      Assert.Equal(1, p.CountSetBits());
      
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(-1, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(8, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => _ = p.Get(8));
   }

   [Fact]
   public void PackedBools16Operations()
   {
      var p = new PackedBools16(0);
      
      Assert.Equal(0, p.RawValue);
      Assert.Equal(0, p.CountSetBits());
      
      p.Set(0, true);
      p.Set(15, true);
      
      Assert.True(p.Get(0));
      Assert.True(p[15]);
      Assert.False(p[8]);
      Assert.Equal(2, p.CountSetBits());
      
      p[15] = false;
      Assert.False(p[15]);
      Assert.Equal(1, p.CountSetBits());
      
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(-1, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(16, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => _ = p.Get(16));
   }

   [Fact]
   public void PackedBools32Operations()
   {
      var p = new PackedBools32(0);
      
      Assert.Equal(0U, p.RawValue);
      Assert.Equal(0, p.CountSetBits());
      
      p.Set(0, true);
      p.Set(31, true);
      
      Assert.True(p.Get(0));
      Assert.True(p[31]);
      Assert.False(p[16]);
      Assert.Equal(2, p.CountSetBits());
      
      p[31] = false;
      Assert.False(p[31]);
      Assert.Equal(1, p.CountSetBits());
      
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(-1, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(32, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => _ = p.Get(32));
   }

   [Fact]
   public void PackedBools64Operations()
   {
      var p = new PackedBools64(0);
      
      Assert.Equal(0UL, p.RawValue);
      Assert.Equal(0, p.CountSetBits());
      
      p.Set(0, true);
      p.Set(63, true);
      
      Assert.True(p.Get(0));
      Assert.True(p[63]);
      Assert.False(p[32]);
      Assert.Equal(2, p.CountSetBits());
      
      p[63] = false;
      Assert.False(p[63]);
      Assert.Equal(1, p.CountSetBits());
      
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(-1, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => p.Set(64, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => _ = p.Get(64));
   }

   [Fact]
   public void Flags128Operations()
   {
      var flags = new Flags128();
      
      Assert.Equal(0, flags.CountSetBits());
      
      flags.Set(0, true);
      flags.Set(63, true);
      flags.Set(64, true);
      flags.Set(127, true);
      
      Assert.True(flags.Get(0));
      Assert.True(flags.Get(63));
      Assert.True(flags.Get(64));
      Assert.True(flags.Get(127));
      Assert.False(flags.Get(32));
      Assert.False(flags.Get(96));
      Assert.Equal(4, flags.CountSetBits());
      
      flags.Set(127, false);
      Assert.False(flags.Get(127));
      Assert.Equal(3, flags.CountSetBits());
      
      flags.SetRawValues(10, 20);
      Assert.Equal(10UL, flags[0].RawValue);
      Assert.Equal(20UL, flags[1].RawValue);
      
      Assert.Throws<ArgumentOutOfRangeException>(() => flags.Set(-1, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => flags.Set(128, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => _ = flags.Get(128));
   }

   [Fact]
   public void Flags256Operations()
   {
      var flags = new Flags256();
      
      Assert.Equal(0, flags.CountSetBits());
      
      flags.Set(0, true);
      flags.Set(63, true);
      flags.Set(128, true);
      flags.Set(255, true);
      
      Assert.True(flags.Get(0));
      Assert.True(flags.Get(63));
      Assert.True(flags.Get(128));
      Assert.True(flags.Get(255));
      Assert.False(flags.Get(32));
      Assert.False(flags.Get(192));
      Assert.Equal(4, flags.CountSetBits());
      
      flags.Set(255, false);
      Assert.False(flags.Get(255));
      Assert.Equal(3, flags.CountSetBits());
      
      flags.SetRawValues(10, 20, 30, 40);
      Assert.Equal(10UL, flags[0].RawValue);
      Assert.Equal(20UL, flags[1].RawValue);
      Assert.Equal(30UL, flags[2].RawValue);
      Assert.Equal(40UL, flags[3].RawValue);
      
      Assert.Throws<ArgumentOutOfRangeException>(() => flags.Set(-1, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => flags.Set(256, true));
      Assert.Throws<ArgumentOutOfRangeException>(() => _ = flags.Get(256));
   }

   [Fact]
   public void EqualityOperations()
   {
      var p1 = new PackedBools8(15);
      var p2 = new PackedBools8(15);
      var p3 = new PackedBools8(16);
      
      Assert.True(p1 == p2);
      Assert.False(p1 == p3);
      Assert.True(p1 != p3);
      Assert.True(p1.Equals(p2));
      
      var f1 = new Flags128();
      var f2 = new Flags128();
      
      f1.SetRawValues(10, 20);
      f2.SetRawValues(10, 20);
      Assert.True(f1 == f2);
      
      f2.SetRawValues(10, 21);
      Assert.True(f1 != f2);
   }
}
