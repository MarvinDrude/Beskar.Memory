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
