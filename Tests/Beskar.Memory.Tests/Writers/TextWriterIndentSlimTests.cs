using System;
using Xunit;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Tests.Writers;

public class TextWriterIndentSlimTests
{
   [Fact]
   public void ConstructorAndIndentation()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      Assert.Equal(' ', writer.IndentCharacter);
      Assert.Equal('\n', writer.NewLineCharacter);
      Assert.Equal(3, writer.IndentSize);
      Assert.Equal(0, writer.CurrentIndentLevel);
      
      writer.Write("Hello");
      writer.UpIndent();
      writer.WriteLine();
      writer.Write("World");
      
      Assert.Equal(1, writer.CurrentIndentLevel);
      Assert.Equal("Hello\n   World", writer.ToString());
      
      writer.DownIndent();
      Assert.Equal(0, writer.CurrentIndentLevel);
      
      writer.Dispose();
   }

   [Fact]
   public void WriteMultiLine()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      writer.UpIndent();
      writer.Write("First\nSecond\nThird", true);
      
      Assert.Equal("   First\n   Second\n   Third", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void HtmlAndMarkdownEncoding()
   {
      Span<char> buffer = stackalloc char[256];
      var writer = new TextWriterIndentSlim(buffer);
      
      writer.WriteHtmlEncoded("<hello> & \"world's\"");
      writer.WriteLine();
      writer.WriteMarkdownUrlEncoded("my link [one] (two) \\escaped");
      
      Assert.Equal("&lt;hello&gt; &amp; &quot;world&#39;s&quot;\nmy%20link%20%5Bone%5D%20%28two%29%20%5Cescaped", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void OperatorOverloading()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      writer += 'A';
      writer += "BC";
      writer <<= 'D';
      writer <<= "EF";
      writer |= "G";
      
      Assert.Equal("ABCDEFG\n", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void InterpolatedStringHandler()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      var value = 42;
      writer.WriteInterpolated($"The answer is {value}.");
      writer.WriteLineInterpolated($" Let's print {100}!");
      
      Assert.Equal("The answer is 42. Let's print 100!\n", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void ConstructorWithSecondary()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      writer.UpIndent();
      writer.UpIndent();
      writer.Write("Test");
      
      Assert.Equal("      Test", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void WriteLineIfCondition()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      writer.WriteLineIf(false);
      writer.WriteLineIf(true);
      writer.WriteLineIf(false, "No");
      writer.WriteLineIf(true, "Yes");
      
      Assert.Equal("\nYes\n", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void WriteIfCondition()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      writer.WriteIf(false, "No");
      writer.WriteIf(true, "Yes");
      
      Assert.Equal("Yes", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void AcquireSpanAndIndented()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      writer.UpIndent();
      
      var span1 = writer.AcquireSpanIndented(2);
      span1[0] = 'A';
      span1[1] = 'B';
      
      writer.WriteLine();
      
      var span2 = writer.AcquireSpanIndented(2);
      span2[0] = 'C';
      span2[1] = 'D';
      
      Assert.Equal("   AB\n   CD", writer.ToString());
      
      writer.Dispose();
   }

   [Fact]
   public void DownIndentBoundaries()
   {
      Span<char> buffer = stackalloc char[128];
      var writer = new TextWriterIndentSlim(buffer);
      
      var threw = false;
      try
      {
         writer.DownIndent();
      }
      catch (ArgumentOutOfRangeException)
      {
         threw = true;
      }
      
      Assert.True(threw);
      
      writer.Dispose();
   }

   [Fact]
   public void HtmlEncodingApostrophe()
   {
      Span<char> buffer1 = stackalloc char[128];
      var writer1 = new TextWriterIndentSlim(buffer1);
      writer1.WriteHtmlEncoded("'", encodeApostrophe: true);
      Assert.Equal("&#39;", writer1.ToString());
      writer1.Dispose();
      
      Span<char> buffer2 = stackalloc char[128];
      var writer2 = new TextWriterIndentSlim(buffer2);
      writer2.WriteHtmlEncoded("'", encodeApostrophe: false);
      Assert.Equal("'", writer2.ToString());
      writer2.Dispose();
   }
}
