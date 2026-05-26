using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Tests.Helpers;

namespace Beskar.Memory.Code.Tests.Common
{
   public class AttributeExtensionsTests
   {
      private const string SourceCode = @"
using System;

[AttributeUsage(AttributeTargets.Class)]
public class MyTestAttribute : Attribute
{
   public MyTestAttribute(string strVal, bool boolVal, int intVal, int[] intArr) {}
   public string NamedStr { get; set; }
   public bool NamedBool { get; set; }
   public double NamedDouble { get; set; }
   public string[] NamedStrArr { get; set; }
   public MyEnum NamedEnum { get; set; }
}

public enum MyEnum
{
   First,
   Second
}

[MyTest(""ConstructorStr"", true, 42, new int[] { 1, 2, 3 }, 
        NamedStr = ""NamedValue"", 
        NamedBool = false, 
        NamedDouble = 3.14, 
        NamedStrArr = new string[] { ""hello"", ""world"" },
        NamedEnum = MyEnum.Second)]
public class MyTargetClass {}
";

      [Fact]
      public void GetParameterValues_ExtractsPositionalArgumentsSuccessfully()
      {
         var attribute = TestCompilationHelper.GetAttributeData(SourceCode, "MyTargetClass");

         // Test positional parameters
         Assert.Equal("ConstructorStr", attribute.GetParameterStringValue(0));
         Assert.True(attribute.GetParameterBoolValue(1));
         Assert.Equal(42, attribute.GetParameterIntValue(2));

         var intArr = attribute.GetParameterIntArrayValues(3);
         Assert.Equal(new[] { 1, 2, 3 }, intArr);

         // Out of bounds fallbacks
         Assert.Equal("Default", attribute.GetParameterStringValue(10, "Default"));
         Assert.False(attribute.GetParameterBoolValue(10, false));
         Assert.Equal(100, attribute.GetParameterIntValue(10, 100));
         Assert.Empty(attribute.GetParameterIntArrayValues(10));
      }

      [Fact]
      public void GetNamedArgumentValues_ExtractsNamedArgumentsSuccessfully()
      {
         var attribute = TestCompilationHelper.GetAttributeData(SourceCode, "MyTargetClass");

         // Test named parameters
         Assert.Equal("NamedValue", attribute.GetNamedStringValueOrDefault("NamedStr"));
         Assert.False(attribute.GetNamedBoolValueOrDefault("NamedBool", true));
         Assert.Equal(3.14, attribute.GetNamedDoubleValueOrDefault("NamedDouble"));

         var strArr = attribute.GetNamedStringArrayValuesOrDefault("NamedStrArr");
         Assert.Equal(new[] { "hello", "world" }, strArr);

         // Enum members extraction
         Assert.Equal("MyEnum.Second", attribute.GetNamedEnumFullNameOrDefault("NamedEnum"));
         
         // Missing argument fallbacks
         Assert.Equal("Fallback", attribute.GetNamedStringValueOrDefault("NonExistent", "Fallback"));
         Assert.True(attribute.GetNamedBoolValueOrDefault("NonExistent", true));
         Assert.Equal(5.5, attribute.GetNamedDoubleValueOrDefault("NonExistent", 5.5));
         Assert.Empty(attribute.GetNamedStringArrayValuesOrDefault("NonExistent"));
      }

      [Fact]
      public void DetermineValue_ChecksNamedFirst_FallsBackToPositional()
      {
         var attribute = TestCompilationHelper.GetAttributeData(SourceCode, "MyTargetClass");

         // NamedStr is specified as "NamedValue"
         Assert.Equal("NamedValue", attribute.DetermineStringValue("NamedStr", 0));

         // StrVal at parameter index 0 is "ConstructorStr" (named parameter "NonExistent" is absent)
         Assert.Equal("ConstructorStr", attribute.DetermineStringValue("NonExistent", 0));

         // Both named and positional absent
         Assert.Equal("Fallback", attribute.DetermineStringValue("NonExistent", 10, "Fallback"));
      }
   }
}
