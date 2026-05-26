# Beskar.Memory

A suite of low-allocation, ultra-high-performance .NET libraries designed to eliminate GC overhead and optimize execution on hot paths.

## Solution Packages

| Package | NuGet | Documentation | Description |
| :--- | :--- | :--- | :--- |
| **`Beskar.Memory`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.svg)](https://www.nuget.org/packages/Beskar.Memory/) | [README](Common/Beskar.Memory/README.md) | Core memory primitives: ring buffers, lock-free pools, hardware-accelerated bitsets, and stack-safe binary readers/writers. |
| **`Beskar.Memory.Code`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.svg)](https://www.nuget.org/packages/Beskar.Memory.Code/) | [README](Common/Beskar.Memory.Code/README.md) | High-performance Roslyn Source Generator helpers: zero-allocation symbol caching, diagnostic mapping, and text-generation. |
| **`Beskar.Memory.Code.TypeIdGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.TypeIdGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.TypeIdGenerator/) | [README](Bundles/Beskar.Memory.Code.TypeIdGenerator/README.md) | High-performance C# incremental source generator for generating type-safe, zero-allocation ID record structs. |
| **`Beskar.Memory.Code.ObserveGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.ObserveGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.ObserveGenerator/) | [README](Bundles/Beskar.Memory.Code.ObserveGenerator/README.md) | High-performance C# incremental source generator for Activity and Metrics telemetry extensions. |
| **`Beskar.Memory.Code.EnumGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.EnumGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.EnumGenerator/) | [README](Bundles/Beskar.Memory.Code.EnumGenerator/README.md) | High-performance C# incremental source generator for fast, zero-allocation enum parsing, defined-checks, and string representation. |
| **`Beskar.Memory.Code.PacketGenerator`** | [![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.PacketGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.PacketGenerator/) | [README](Bundles/Beskar.Memory.Code.PacketGenerator/README.md) | High-performance C# incremental source generator for high-performance packet serialization and registry routing. |

## Verification & Testing

The stability and performance of the libraries are verified by a comprehensive xUnit test suite containing **247** unit tests covering all core memory utilities, Roslyn helpers, and compiler-host incremental source generators:
* **Core Memory Utilities (`Beskar.Memory.Tests`)**: **195** unit tests.
* **Source Generator Common Helpers (`Beskar.Memory.Code.Tests`)**: **47** unit tests.
* **TypeIdGenerator (`Beskar.Memory.Code.TypeIdGenerator.Tests`)**: **1** compiler-host validation test scenario.
* **ObserveGenerator (`Beskar.Memory.Code.ObserveGenerator.Tests`)**: **1** compiler-host validation test scenario.
* **EnumGenerator (`Beskar.Memory.Code.EnumGenerator.Tests`)**: **1** compiler-host validation test scenario.
* **PacketGenerator (`Beskar.Memory.Code.PacketGenerator.Tests`)**: **2** compiler-host validation test scenarios.

