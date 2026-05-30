using Beskar.Memory.Code.EnumGenerator.Attributes;

namespace Beskar.Memory.Code.EnumGenerator.Tests.Scenarios.Simple;

[FastEnum]
public enum TestEnum : long
{
   First = 1,
   Second = 2,
   Third,
}

[FastEnum]
public enum ByteEnum : byte
{
   First = 1,
   Example
}

[FastEnum]
public enum LocalizedEnum
{
   [System.ComponentModel.DataAnnotations.Display(Name = "StandardName")]
   Standard,

   [System.ComponentModel.DataAnnotations.Display(Name = "ShippedKey", ResourceType = typeof(MockResources))]
   Shipped,
   
   None
}

public static class MockResources
{
   public static string ShippedKey => "Package Shipped";
}
