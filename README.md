# Beskar.Memory

A suite of low-allocation, ultra-high-performance .NET libraries designed to eliminate GC overhead and optimize execution on hot paths.

## Solution Packages

| Package | NuGet | Documentation | Description |
| :--- | :--- | :--- | :--- |
| **`Beskar.Memory`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.svg)](https://www.nuget.org/packages/Beskar.Memory/) | [README](Common/Beskar.Memory/README.md) | Core memory primitives: ring buffers, lock-free pools, hardware-accelerated bitsets, and stack-safe binary readers/writers. |
| **`Beskar.Memory.Code`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.svg)](https://www.nuget.org/packages/Beskar.Memory.Code/) | [README](Common/Beskar.Memory.Code/README.md) | High-performance Roslyn Source Generator helpers: zero-allocation symbol caching, diagnostic mapping, and text-generation. |
| **`Beskar.Memory.Code.TypeIdGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.TypeIdGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.TypeIdGenerator/) | [README](Bundles/Beskar.Memory.Code.TypeIdGenerator/README.md) | High-performance C# incremental source generator for generating type-safe, zero-allocation ID record structs. |
