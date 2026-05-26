# Beskar.Memory.Code.PacketGenerator

[![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.PacketGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.PacketGenerator/)

A high-performance C# incremental source generator that generates high-performance packet serialization and routing extensions backed by `Beskar.Memory`.

---

## Core Component Directory

### 1. Attributes & Configuration

* **`PacketAttribute`**
   * *Description*: Marks a class or struct to trigger incremental code generation of packet serialization/telemetry metadata.
* **`PacketRegistryAttribute`**
   * *Description*: Marks a class to trigger incremental code generation of packet routing and dispatching registries.
