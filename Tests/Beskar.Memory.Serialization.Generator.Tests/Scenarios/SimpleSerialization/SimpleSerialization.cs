using System;
using Beskar.Memory.Serialization.Attributes;

namespace TestNamespace;

[BeskarObject]
public partial class SimpleClass
{
   [BeskarOrder(0)]
   public int Id { get; set; }

   [BeskarOrder(1)]
   public string Name { get; set; } = string.Empty;

   [BeskarOrder(2)]
   public int Id2 { get; set; }
}

[BeskarObject]
public partial struct SimpleStruct
{
   [BeskarOrder(0)]
   public double Value { get; set; }
}

[BeskarObject]
public partial class IgnoredMemberClass
{
   [BeskarOrder(0)]
   public int Kept { get; set; }

   [BeskarIgnore]
   public int Ignored { get; set; }

   [BeskarIgnore]
   [BeskarOrder(1)]
   public int IgnoredWithOrder { get; set; }
}

[BeskarObject]
public partial record ImmutableRecord(
   [property: BeskarOrder(0)] int Id,
   [property: BeskarOrder(1)] string Data
);

[BeskarObject]
[BeskarUnion(1, typeof(UnionChild))]
public partial class UnionBase
{
   [BeskarOrder(0)]
   public int BaseValue { get; set; }
}

[BeskarObject]
public partial class UnionChild : UnionBase
{
   [BeskarOrder(1)]
   public string ChildValue { get; set; } = string.Empty;
}

[BeskarObject]
public partial class NonPolyBase
{
   [BeskarOrder(0)]
   public int BaseValue { get; set; }
}

[BeskarObject]
public partial class NonPolyChild : NonPolyBase
{
   [BeskarOrder(1)]
   public int ChildValue { get; set; }
}

[BeskarObject]
public class NonPartialClass
{
   [BeskarOrder(0)]
   public int Id { get; set; }
}

[BeskarObject]
public partial class TypeWithReadonlyMembers
{
   [BeskarOrder(0)]
   public int Value { get; set; }

   public readonly int ReadonlyField = 42;
   public int ReadonlyProp => Value + 10;
}

[BeskarObject]
public struct GenericPacketWithConstraints<TPacket> where TPacket : struct
{
   [BeskarOrder(0)]
   public TPacket Packet { get; set; }
}
