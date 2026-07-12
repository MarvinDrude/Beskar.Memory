<h1>
<p align="center">
   <img src="https://github.com/MarvinDrude/Beskar.Memory/blob/master/Resources/banner.png" alt="Logo" width="128" />
   <br />
   Beskar.Memory
</p>
</h1>
<p align="center">
   Low-allocation, ultra-high-performance memory primitives, serialization, and utility libraries for .NET.<br/>
   Designed to eliminate GC overhead and optimize execution on hot execution paths.<br/>
   No external runtime dependencies.<br/><br/>
   <a href="#about">About</a>
   ·
   <a href="#solution-packages">Solution Packages</a>
   ·
   <a href="#key-features">Key Features</a>
   ·
   <a href="#verification--testing">Verification & Testing</a>
   ·
   <a href="#️-roadmap--active-work">Roadmap</a>
</p>

---
<br/>

## About

`Beskar.Memory` is a suite of low-allocation, high-performance C# utility libraries designed to minimize GC pressure and maximize CPU-cache efficiency in hot paths. It provides lock-free object pools, high-performance ring buffers, hardware-accelerated bitsets, zero-allocation binary readers/writers, and Roslyn-powered incremental source generators for high-speed serialization and utility parsing.

---

## Solution Packages

| Package | NuGet | Documentation | Description |
| :--- | :--- | :--- | :--- |
| **`Beskar.Memory`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.svg)](https://www.nuget.org/packages/Beskar.Memory/) | [README](Common/Beskar.Memory/README.md) | Core memory primitives: ring buffers, lock-free pools, hardware-accelerated bitsets, and stack-safe binary readers/writers. |
| **`Beskar.Memory.Code`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.svg)](https://www.nuget.org/packages/Beskar.Memory.Code/) | [README](Common/Beskar.Memory.Code/README.md) | High-performance Roslyn Source Generator helpers: zero-allocation symbol caching, diagnostic mapping, and text-generation. |
| **`Beskar.Memory.Serialization`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Serialization.svg)](https://www.nuget.org/packages/Beskar.Memory.Serialization/) | [README](Common/Beskar.Memory.Serialization/README.md) | Zero-allocation, ultra-high-performance binary serialization engine utilizing C# 12 static abstract interface members and compile-time source generation. |
| **`Beskar.Memory.Code.TypeIdGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.TypeIdGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.TypeIdGenerator/) | [README](Bundles/Beskar.Memory.Code.TypeIdGenerator/README.md) | High-performance C# incremental source generator for generating type-safe, zero-allocation ID record structs. |
| **`Beskar.Memory.Code.ObserveGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.ObserveGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.ObserveGenerator/) | [README](Bundles/Beskar.Memory.Code.ObserveGenerator/README.md) | High-performance C# incremental source generator for Activity and Metrics telemetry extensions. |
| **`Beskar.Memory.Code.EnumGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.EnumGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.EnumGenerator/) | [README](Bundles/Beskar.Memory.Code.EnumGenerator/README.md) | High-performance C# incremental source generator for fast, zero-allocation enum parsing, defined-checks, and string representation. |
| **`Beskar.Memory.Code.PacketGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.PacketGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.PacketGenerator/) | [README](Bundles/Beskar.Memory.Code.PacketGenerator/README.md) | High-performance C# incremental source generator for high-performance packet serialization and registry routing. |

---

## Key Features

- **Zero & Low Allocations**: Minimizes garbage collection pressure via reusable array-backed rentals (`SpanOwner<T>`, `MemoryOwner<T>`), circular buffers, and highly optimized object pools.
- **Hardware Acceleration**: Employs CPU intrinsics and bitwise flag structures (`PackedBools`, `Flags128`, `Flags256` utilizing C# 12 `[InlineArray]`) for O(1) segment lookups and branchless operations.
- **High-Performance Object Pooling**: Concurrent thread-safe pools (`ObjectPool<T>`) featuring lock-free fast paths via atomic `Interlocked` operations to bypass queue contention.
- **Compile-Time Generators**: Incremental source generators for fast type-safe record IDs, lightning-fast enum parsers, zero-allocation telemetry hooks, and packet registry codegen.

---

## Verification & Testing

The stability and performance of the libraries are verified by a comprehensive xUnit test suite containing **247** unit tests covering all core memory utilities, Roslyn helpers, and compiler-host incremental source generators:

* **Core Memory Utilities (`Beskar.Memory.Tests`)**: **195** unit tests.
* **Source Generator Common Helpers (`Beskar.Memory.Code.Tests`)**: **47** unit tests.
* **TypeIdGenerator (`Beskar.Memory.Code.TypeIdGenerator.Tests`)**: **1** compiler-host validation test scenario.
* **ObserveGenerator (`Beskar.Memory.Code.ObserveGenerator.Tests`)**: **1** compiler-host validation test scenario.
* **EnumGenerator (`Beskar.Memory.Code.EnumGenerator.Tests`)**: **1** compiler-host validation test scenario.
* **PacketGenerator (`Beskar.Memory.Code.PacketGenerator.Tests`)**: **2** compiler-host validation test scenarios.

---

## 🗺️ Roadmap & Active Work

- [x] High-performance Circular Buffers (`CircularBuffer<T>`, `CircularBufferSlim<T>`)
- [x] Lock-free and concurrent object pools (`ObjectPool<T>`, `DisposableObjectPool<T>`, `AsyncDisposableObjectPool<T>`)
- [x] Hardware-accelerated bitwise structures (`PackedBools8/16/32/64`, `Flags128/256`)
- [x] High-speed C# Roslyn incremental source generators (`TypeIdGenerator`, `EnumGenerator`, `ObserveGenerator`, `PacketGenerator`)
- [x] Zero-allocation binary readers and stack-safe writers (`ByteReader`, `ByteWriter`, `BufferWriter<T>`)
- [ ] Dynamic performance benchmarking suites
- [ ] Comprehensive documentation and integration examples
