# Beskar.Memory.Code

[![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.Code.svg)](https://www.nuget.org/packages/Beskar.Memory.Code/)

A high-performance utility library designed for Roslyn Source Generators. It provides low-allocation abstractions to analyze compiler symbols, build diagnostic maps, and generate code cleanly and rapidly. It replaces standard heavy LINQ-based symbol operations with low-allocation structures backed by `Beskar.Memory`.

---

## Core Component Directory

### 1. Code Rendering & Formatting

* **`CodeTextWriter`**
   * *Description*: A high-performance, stack-only `ref struct` writer designed specifically for code generation.
   * *Key Feature*: Integrates automatic, custom-configurable indentation levels and interpolated string formatting with zero heap allocations.
* **`CodeRenderer`**
   * *Description*: A simple abstract base class providing standardized, cancellation-aware C# source file creation using Roslyn's `SourceProductionContext`.
* **`CodeWriterExtensions`**
   * *Description*: An extension block on `ref CodeTextWriter` providing fast formatting templates like `WriteEnableNullable`, `WriteUsing`, `WriteNamespace`, and auto-generated header blocks.
* **`SymbolDisplayFormats`**
   * *Description*: A collection of static Roslyn display format templates used to generate clean type, member, and property signature strings.

### 2. Diagnostics Management

* **`DiagnosticBuilder<T>`**
   * *Description*: A high-performance, low-allocation builder designed to accumulate, format, and dispatch compiler diagnostics.
   * *Key Feature*: Backed by `ArrayBuilder<T>` to completely eliminate garbage collection overhead in compiler analysis hot paths.
* **`DiagnosticSpec`**
   * *Description*: A readonly record representing a specific diagnostic code and its parameters.
* **`MaybeSpec<T>`**
   * *Description*: A union spec representing the optional successfully compiled code generator value alongside accumulated compiler diagnostics.

### 3. Roslyn Symbol Specifications (Specs)

Lightweight specification records that serialize compiler symbols into easily matching models:

* **`SymbolSpec`**
   * *Description*: The base record holding name, accessibility, and flag indicators for any compiler symbol.
* **`TypeSymbolSpec`**
   * *Description*: A specification detail representing unmanaged or managed compiler types.
* **`NamedTypeSymbolSpec`**
   * *Description*: Representing classes, records, and interfaces with nested specifications.
* **`FieldSymbolSpec`**
   * *Description*: Specification maps for class fields and constants.
* **`PropertySymbolSpec`**
   * *Description*: Representing properties including custom get/set configurations.
* **`MethodSymbolSpec`**
   * *Description*: Model representing void returns, parameters, attributes, and asynchronous/iterator states.
* **`ParameterSymbolSpec`**
   * *Description*: Parameter specifications mapped inside method signatures.
* **`TypeParameterSymbolSpec`**
   * *Description*: Generic type constraints and parameters.

### 4. Roslyn Metadata Archetypes

Compact metadata models used to generate raw symbol signatures during code rendering:

* **`TypeSymbolArchetype`** / **`NamedTypeSymbolArchetype`** / **`FieldSymbolArchetype`** / **`PropertySymbolArchetype`** / **`MethodSymbolArchetype`** / **`ParameterSymbolArchetype`** / **`TypeParameterArchetype`**
   * *Description*: A clean hierarchy of lightweight, highly-cached symbol description structures.

### 5. transformers & Translators

* **`SymbolSpecTransformer`** / **`TypeSymbolSpecTransformer`** / **`NamedTypeSymbolSpecTransformer`** / **`FieldSymbolSpecTransformer`** / **`PropertySymbolSpecTransformer`** / **`MethodSymbolSpecTransformer`** / **`ParameterSymbolSpecTransformer`** / **`TypeParameterSymbolSpecTransformer`**
   * *Description*: Systemic translation helpers that compile active Roslyn compiler symbols (`ISymbol`, `IMethodSymbol`, etc.) into specification specs under custom filters.
* **`TypeSymbolArchetypeTransformer`** / **`NamedTypeSymbolArchetypeTransformer`** / **`FieldSymbolArchetypeTransformer`** / **`PropertySymbolArchetypeTransformer`** / **`MethodSymbolArchetypeTransformer`** / **`ParameterSymbolArchetypeTransformer`** / **`TypeParameterSymbolArchetypeTransformer`**
   * *Description*: Transformers converting active symbols into their lightweight description archetypes.

### 6. Extensions

* **`AttributeDataExtensions`**
   * *Description*: Provides high-performance extension members on Roslyn's `AttributeData` to cleanly extract named arguments (numbers, booleans, enums, arrays, and types) without memory allocations.
* **`AttributeDataConstructorExtensions`**
   * *Description*: Helpers to extract positional arguments from attribute constructor calls.
* **`AttributeDataFallbackExtensions`**
   * *Description*: Fallback value wrappers for named arguments.
* **`SourceProductionContextExtensions`**
   * *Description*: Dispatches diagnostic lists to Roslyn contexts utilizing zero-allocation arrays.
* **`TypedConstantExtensions`** / **`TypedConstantArrayExtensions`**
   * *Description*: Extensions to parse TypedConstant values and arrays efficiently.
* **`SymbolExtensions`** / **`TypeSymbolExtensions`** / **`NamedTypeSymbolExtensions`** / **`FieldSymbolExtensions`** / **`PropertySymbolExtensions`** / **`MethodSymbolExtensions`** / **`ParameterSymbolExtensions`** / **`TypeParameterSymbolExtensions`**
   * *Description*: Low-allocation shortcut helpers for typical symbol operations.
* **`StringExtensions`**
   * *Description*: Performs high-performance casing transitions (such as `SnakeCase` or `FirstCharToLower`) backed by `SpanOwner<char>`.
* **`AccessibilityExtensions`**
   * *Description*: Maps C# Accessibility to standard keyword string signatures (`public`, `private`, etc.).
