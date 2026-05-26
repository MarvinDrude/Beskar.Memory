using System.Linq;
using Xunit;
using Beskar.Memory.Code.Common.System;

namespace Beskar.Memory.Code.Tests.Common
{
   public class StringExtensionsTests
   {
      [Fact]
      public void FirstCharToLower_EmptyString_ReturnsSame()
      {
         Assert.Equal("", "".FirstCharToLower());
      }

      [Fact]
      public void FirstCharToLower_AlreadyLowercase_ReturnsSame()
      {
         Assert.Equal("hello", "hello".FirstCharToLower());
      }

      [Fact]
      public void FirstCharToLower_UppercaseFirstChar_LowercasesFirstCharOnly()
      {
         Assert.Equal("helloWorld", "HelloWorld".FirstCharToLower());
      }

      [Fact]
      public void FirstCharToLower_SingleCharUppercase_ReturnsLowercase()
      {
         Assert.Equal("a", "A".FirstCharToLower());
      }

      [Fact]
      public void FirstCharToUpper_EmptyString_ReturnsSame()
      {
         Assert.Equal("", "".FirstCharToUpper());
      }

      [Fact]
      public void FirstCharToUpper_AlreadyUppercase_ReturnsSame()
      {
         Assert.Equal("HELLO", "HELLO".FirstCharToUpper());
      }

      [Fact]
      public void FirstCharToUpper_LowercaseFirstChar_UppercasesFirstCharOnly()
      {
         Assert.Equal("HelloWorld", "helloWorld".FirstCharToUpper());
      }

      [Fact]
      public void FirstCharToUpper_SingleCharLowercase_ReturnsUppercase()
      {
         Assert.Equal("A", "a".FirstCharToUpper());
      }

      [Fact]
      public void SnakeCase_NullOrEmptyString_ReturnsSame()
      {
         Assert.Null(((string)null!).SnakeCase());
         Assert.Equal("", "".SnakeCase());
      }

      [Fact]
      public void SnakeCase_SingleWordLowercase_ReturnsLowercase()
      {
         Assert.Equal("hello", "hello".SnakeCase());
      }

      [Fact]
      public void SnakeCase_CamelCase_InsertsUnderscoresAndLowercases()
      {
         Assert.Equal("hello_world", "helloWorld".SnakeCase());
         Assert.Equal("hello_world_again", "helloWorldAgain".SnakeCase());
      }

      [Fact]
      public void SnakeCase_PascalCase_InsertsUnderscoresAndLowercases()
      {
         Assert.Equal("hello_world", "HelloWorld".SnakeCase());
         Assert.Equal("hello_world_again", "HelloWorldAgain".SnakeCase());
      }

      [Fact]
      public void SnakeCase_LongString_TriggersArrayPoolBranch_Succeeds()
      {
         // A string of 300 characters will have maxLength = 600, exceeding 512, which triggers the heap-allocated SpanOwner
         var longPascalString = string.Concat(Enumerable.Repeat("HelloWorld", 30)); // 300 chars
         var expectedSnakeString = string.Join("_", Enumerable.Repeat("hello_world", 30)).Replace("d_h", "d_h"); // Wait, HelloWorldHelloWorld -> hello_world_hello_world
         
         // Let's build expected manually to be precise:
         // "HelloWorldHelloWorld" -> "hello_world_hello_world"
         // So for 30 repetitions, it should have "hello_world" repeated 30 times joined with '_'
         var expected = string.Join("_", Enumerable.Repeat("hello_world", 30));
         
         Assert.Equal(expected, longPascalString.SnakeCase());
      }
   }
}
