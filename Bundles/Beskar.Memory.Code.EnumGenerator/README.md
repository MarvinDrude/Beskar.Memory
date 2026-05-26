# Beskar.Memory.Code.EnumGenerator

[![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.EnumGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.EnumGenerator/)

A high-performance C# incremental source generator that generates high-performance enum helper extensions backed by `Beskar.Memory`.

---

## Core Component Directory

### 1. Attributes & Configuration

* **`FastEnumAttribute`**
   * *Description*: Marks an enum to trigger incremental code generation of zero-allocation string parsing, representation, and validation helpers.
