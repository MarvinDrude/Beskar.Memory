using System;
using Xunit;
using Beskar.Memory.Code;

namespace Beskar.Memory.Tests.Code;

public class CodeTextWriterTests
{
   [Fact]
   public void ConstructorAndBodyFormatting()
   {
      Span<char> buffer = stackalloc char[256];
      var writer = new CodeTextWriter(buffer);
      
      writer.Write("public class Foo");
      writer.OpenBody();
      writer.Write("public void Bar()");
      writer.OpenBody();
      writer.WriteLine("int x = 42;");
      writer.CloseBody();
      writer.CloseBodySemicolon();
      
      var expected = "public class Foo{\n   public void Bar(){\n      int x = 42;\n   }\n};\n";
      Assert.Equal(expected, writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void WritingAPIs()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new CodeTextWriter(buffer);
      
      writer.Write("Hello");
      writer.WriteIf(false, "No");
      writer.WriteIf(true, " World");
      writer.WriteLine();
      writer.WriteLineIf(false, "No");
      writer.WriteLineIf(true, "Yes");
      
      Assert.Equal("Hello World\nYes\n", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void Operators()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new CodeTextWriter(buffer);
      
      writer += 'A';
      writer += "BC";
      writer <<= 'D';
      writer <<= "EF";
      writer |= "G";
      
      Assert.Equal("ABCDEFG\n", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void Interpolation()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new CodeTextWriter(buffer);
      
      var name = "Beskar";
      writer.WriteInterpolated($"Library: {name}");
      writer.WriteLineInterpolated($" Version: {10}");
      
      Assert.Equal("Library: Beskar Version: 10\n", writer.ToString());
      
      writer.Dispose();
   }
}
