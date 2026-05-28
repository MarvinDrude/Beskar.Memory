using Beskar.Memory.Serialization.Attributes;

namespace Beskar.Memory.Benchmarks.Live;

[BeskarObject]
public partial class SimpleClass
{
    [BeskarOrder(0)] public int Id { get; set; }
    [BeskarOrder(1)] public string Name { get; set; } = string.Empty;
    [BeskarOrder(2)] public int Id2 { get; set; }
}

[BeskarObject]
public partial class MediumClass
{
    [BeskarOrder(0)] public int Id { get; set; }
    [BeskarOrder(1)] public string Name { get; set; } = string.Empty;
    [BeskarOrder(2)] public List<int> Numbers { get; set; } = new();
}

[BeskarObject]
public partial class VeryComplexClass
{
    [BeskarOrder(0)] public int Id { get; set; }
    [BeskarOrder(1)] public string Name { get; set; } = string.Empty;
    [BeskarOrder(2)] public List<string> Tags { get; set; } = new();
    [BeskarOrder(3)] public List<SimpleClass> Children { get; set; } = new();
    [BeskarOrder(4)] public List<MediumClass> SimpleChildren { get; set; } = new();
}
