using BenchmarkDotNet.Attributes;
using Beskar.Memory.Serialization;
using Beskar.Memory.Serialization.Attributes;

namespace Beskar.Memory.Benchmarks;

[MemoryDiagnoser]
public class ThroughputBenchmarks
{
    private SimpleClass _simpleClass = null!;
    private ComplexClass _complexClass = null!;
    
    private byte[] _simpleClassSerialized = null!;
    private byte[] _complexClassSerialized = null!;
    
    private byte[] _serializationBuffer = null!;

    [GlobalSetup]
    public void Setup()
    {
        _simpleClass = new SimpleClass
        {
            Id = 42,
            Name = "John Doe",
            Id2 = 100
        };

        _complexClass = new ComplexClass
        {
            Id = 1,
            Name = "Root Complex Object",
            Tags = new List<string> { "Benchmark", "Throughput", "Beskar", "Memory", "Serializer" },
            Items = new List<SimpleClass>
            {
                new SimpleClass { Id = 10, Name = "Nested One", Id2 = 20 },
                new SimpleClass { Id = 30, Name = "Nested Two", Id2 = 40 }
            }
        };

        _simpleClassSerialized = BeSerializer.Serialize(_simpleClass);
        _complexClassSerialized = BeSerializer.Serialize(_complexClass);
        
        // Large enough buffer for reuse to test zero-allocation serialization throughput
        _serializationBuffer = new byte[8192];
    }

    [Benchmark]
    public byte[] SerializeSimpleClass_ToArray()
    {
        return BeSerializer.Serialize(_simpleClass);
    }

    [Benchmark]
    public int SerializeSimpleClass_ToSpan()
    {
        return BeSerializer.Serialize(_simpleClass, _serializationBuffer);
    }

    [Benchmark]
    public SimpleClass DeserializeSimpleClass()
    {
        return BeSerializer.Deserialize<SimpleClass>(_simpleClassSerialized);
    }

    [Benchmark]
    public byte[] SerializeComplexClass_ToArray()
    {
        return BeSerializer.Serialize(_complexClass);
    }

    [Benchmark]
    public int SerializeComplexClass_ToSpan()
    {
        return BeSerializer.Serialize(_complexClass, _serializationBuffer);
    }

    [Benchmark]
    public ComplexClass DeserializeComplexClass()
    {
        return BeSerializer.Deserialize<ComplexClass>(_complexClassSerialized);
    }
}

[BeskarObject]
public partial class SimpleClass
{
    [BeskarOrder(0)] public int Id { get; set; }
    [BeskarOrder(1)] public string Name { get; set; } = string.Empty;
    [BeskarOrder(2)] public int Id2 { get; set; }
}

[BeskarObject]
public partial class ComplexClass
{
    [BeskarOrder(0)] public int Id { get; set; }
    [BeskarOrder(1)] public string Name { get; set; } = string.Empty;
    [BeskarOrder(2)] public List<string> Tags { get; set; } = new();
    [BeskarOrder(3)] public List<SimpleClass> Items { get; set; } = new();
}
