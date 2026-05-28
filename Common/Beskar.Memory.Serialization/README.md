# Beskar.Memory.Serialization

[![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Serialization.svg)](https://www.nuget.org/packages/Beskar.Memory.Serialization/)

A modern, suite-integrated, zero-allocation, ultra-high-performance binary serialization engine for .NET. By combining Roslyn source generation, C# 12 static abstract interface members, and variable-length integer compaction, `Beskar.Memory.Serialization` achieves blazing-fast execution speeds and near-zero garbage collection overhead.

---

## Key Features

* **Zero-Overhead Static Abstract Interfaces**: Built on `ISerializer<T>` with static abstract interface methods, completely bypassing virtual dispatch and interface object allocation overhead. Fully compatible with the C# 12 `allows ref struct` anti-constraint.
* **Incremental Roslyn Source Generator (`[BeskarObject]`)**: Automatically generates highly-optimized serializers for your custom classes and structs at compile time. Eliminates runtime reflection and startup latency entirely.
* **Variable-Length Integer Encoding (`VarInteger`)**:
  * Uses **LEB128 Varint** compaction for unsigned types (`uint`, `ulong`) and length headers (arrays, lists, strings).
  * Employs **ZigZag mapping** for signed types (`int`, `long`) to compact small negative numbers (e.g., `-1` encodes into `1` byte instead of `10` bytes).
* **Comprehensive Type Support**: Out-of-the-box support for primitive types, enums, nullables, `Guid`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `DateOnly`, `TimeOnly`, `Uri`, collections (`T[]`, `List<T>`), and polymorphic unions (`[BeskarUnion]`).
* **Ultra-Fast Stream & Memory Pipelines**: Integrates deeply with modern high-performance .NET memory types: `BufferWriter<byte>`, `SequenceReader<byte>`, `ReadOnlySequence<byte>`, and stack-allocated spans.

---

## Architecture Overview

At the heart of the library is the `ISerializer<T>` interface:

```csharp
public interface ISerializer<T> : ISerializer
   where T : allows ref struct
{
   public static abstract int Write(ref BufferWriter<byte> writer, scoped in T value);

   public static abstract bool TryRead(ref SequenceReader<byte> reader, [MaybeNullWhen(false)] out T value);

   public static abstract int CalculateByteLength(scoped in T value);
}
```

By leveraging `static abstract` methods, the compiler resolves the serializer call paths statically at compile time, allowing aggressive inlining and zero-overhead dispatch.

---

## Getting Started

### 1. Annotate your Data Objects
Decorate your classes or structs with `[BeskarObject]` and declare them as `partial`. Use `[BeskarOrder(index)]` to define a deterministic serialization order:

```csharp
using Beskar.Memory.Serialization.Attributes;

[BeskarObject]
public partial struct PlayerPosition
{
    [BeskarOrder(0)]
    public int PlayerId { get; set; }

    [BeskarOrder(1)]
    public float X { get; set; }

    [BeskarOrder(2)]
    public float Y { get; set; }

    [BeskarOrder(3)]
    public string? StatusMessage { get; set; }
}
```

### 2. Standard Usage (`BeSerializer`)
The `BeSerializer` static utility class provides highly optimized, convenient methods for serialization and deserialization:

#### Serialization

```csharp
var position = new PlayerPosition
{
    PlayerId = 1337,
    X = 12.5f,
    Y = 88.2f,
    StatusMessage = "Active"
};

// 1. Serialize to a fresh byte array (uses a 256-byte stackalloc buffer for single-pass fast paths)
byte[] bytes = BeSerializer.Serialize(position);

// 2. Serialize directly into an existing Span<byte> (zero-allocation)
Span<byte> buffer = stackalloc byte[128];
int bytesWritten = BeSerializer.Serialize(position, buffer);

// 3. Serialize into a high-performance BufferWriter<byte>
var writer = new BufferWriter<byte>(buffer);
BeSerializer.Serialize(position, ref writer);
```

#### Deserialization

```csharp
// 1. Deserialize from a byte array or ReadOnlySpan<byte>
PlayerPosition player = BeSerializer.Deserialize<PlayerPosition>(bytes);

// 2. Try-deserialization style (defensive parsing)
if (BeSerializer.TryDeserialize<PlayerPosition>(bytes, out var result))
{
    // Use deserialized object...
}
```

---

## Versioning & Polymorphic Unions

You can easily handle polymorphism using the `[BeskarUnion]` attribute, which automatically includes a tag prefix to safely multiplex and demultiplex derived or alternative types:

```csharp
[BeskarObject]
[BeskarUnion(1, typeof(TextMessage))]
[BeskarUnion(2, typeof(ImageMessage))]
public abstract partial class NetworkMessage;

[BeskarObject]
public partial class TextMessage : NetworkMessage
{
    [BeskarOrder(0)]
    public string Content { get; set; } = string.Empty;
}

[BeskarObject]
public partial class ImageMessage : NetworkMessage
{
    [BeskarOrder(0)]
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
}
```
