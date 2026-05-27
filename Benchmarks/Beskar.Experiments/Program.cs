using Beskar.Memory.Serialization.Attributes;

Console.WriteLine("Hello, World!");

return;

[BeskarObject]
public partial class SimpleClass
{
   [BeskarOrder(0)]
   public int Id { get; set; }

   [BeskarOrder(1)]
   public string Name { get; set; } = string.Empty;
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
