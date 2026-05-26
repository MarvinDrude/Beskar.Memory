# Beskar.Memory.Code.TypeIdGenerator

[![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.TypeIdGenerator.svg)](https://www.nuget.org/packages/Beskar.Memory.Code.TypeIdGenerator/)

A high-performance C# incremental source generator that generates type-safe ID record structs (e.g., strong-typed wrappers for integers, GUIDs, and other unmanaged primitives) to prevent domain-level ID mixing bugs. The generated implementations are zero-allocation, extremely fast, and integrate with `Beskar.Memory`'s performance primitives.

---

## Core Component Directory

### 1. Attributes & Configuration

* **`TypeSafeIdAttribute`**
   * *Description*: Marker attribute applied to record structs to trigger incremental code generation of strongly-typed identifiers.
   * *Parameters*:
     * `isOverrideString` (default `true`): Generates custom string representation.
     * `addImplicitConversions` (default `true`): Adds implicit operators to easily cast primitive values to their type-safe counterpart.
     * `addExplicitConversions` (default `true`): Adds explicit operators to retrieve the underlying value.
     * `isSpanParsable` (default `true`): Automatically implements `ISpanParsable<TSelf>` if the underlying type is parsable.
     * `addJsonConverter` (default `true`): Generates a custom, high-performance `System.Text.Json.Serialization.JsonConverter` to serialize and deserialize the ID seamlessly.

### 2. Interfaces & Types

* **`ITypeSafeIdentifier<TUnderlying>`**
   * *Description*: The base interface implemented by all generated type-safe identifiers, exposing the underlying value.
   * *Key Feature*: Enforces clean type safety, `IComparable`, and `IEquatable` across domain identifiers.

### 3. Serialization & Converters

* **`TypeSafeIdJsonConverter<TId, TUnderlying>`**
   * *Description*: An abstract base class for JSON converters to serialize and deserialize type-safe identifiers efficiently.
   * *Key Feature*: Prevents reflection-based JSON overhead, yielding zero heap-allocations during standard JSON reads and writes.
