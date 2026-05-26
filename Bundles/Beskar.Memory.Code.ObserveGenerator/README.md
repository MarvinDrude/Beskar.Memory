# Beskar.Memory.Code.ObserveGenerator

[![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.ObserveGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.ObserveGenerator/)

A high-performance C# incremental source generator that generates high-performance activity and metrics logging extensions backed by `Beskar.Memory`.

---

## Core Component Directory

### 1. Attributes & Configuration

* **`ObserveActivityAttribute`**
   * *Description*: Marks a class to trigger incremental code generation of observability activity/telemetry extensions.
* **`ObserveInstrumentAttribute`**
   * *Description*: Specifies a metrics instrument to log automatically on the observed type.
* **`ObserveMeterAttribute`**
   * *Description*: Marks a class to trigger metrics/meter telemetry extensions.
