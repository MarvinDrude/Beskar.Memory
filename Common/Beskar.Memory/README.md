# Beskar.Memory

[![NuGet Version](https://img.shields.io/nuget/v/Beskar.Memory.svg)](https://www.nuget.org/packages/Beskar.Memory/)

A low-allocation, high-performance C# utility library designed to minimize GC pressure and maximize CPU-cache efficiency in hot execution paths. It provides high-performance ring buffers, lock-free object pools, hardware-accelerated bitsets, stack-safe buffer writers, and zero-allocation binary readers/writers.

---

## Core Component Directory

### 1. Buffers & Writers

* **`CircularBuffer<T>`**
   * *Description*: A high-performance, modulo-free ring buffer backed by `MemoryOwner<T>` rented from a shared array pool.
   * *Key Feature*: Replaces slow division operations (`% Capacity`) with ultra-fast conditional branch subtractions for maximum speed.
* **`CircularBufferSlim<T>`**
   * *Description*: A lightweight, stack-allocated equivalent of `CircularBuffer<T>` for short-lived scopes.
* **`ByteReader`**
   * *Description*: A stack-only, zero-allocation `ref struct` sequence-aware reader.
   * *Key Feature*: Deserializes built-in C# types from contiguous `ReadOnlySpan<byte>` or fragmented `ReadOnlySequence<byte>` arrays.
* **`ByteWriter`**
   * *Description*: A stack-only, zero-allocation `ref struct` writer designed for writing bytes and binary data.
* **`BufferWriter<T>`**
   * *Description*: A premium, stack-only `ref struct` writer that writes to an initial stack-allocated span and seamlessly grows onto rented array pool memory if capacity is exceeded.
* **`SpanOwner<T>`**
   * *Description*: A stack-safe unmanaged/rented buffer owner that wraps disposable array rents.
* **`MemoryOwner<T>`**
   * *Description*: A heap-allocated rented memory block owner with automatic disposal lifecycle tracking.

### 2. Object Pooling

* **`ObjectPool<T>`**
   * *Description*: A thread-safe, concurrent object recycler for heavy reuse scenarios.
   * *Key Feature*: Features a lock-free fast-path head slot using `Interlocked.CompareExchange`, completely bypassing queue contention.
* **`DisposableObjectPool<T>`**
   * *Description*: A specialized object pool for objects implementing `IDisposable`, ensuring proper element disposal upon pool cleanup or overflow.
* **`AsyncDisposableObjectPool<T>`**
   * *Description*: A specialized object pool supporting objects implementing `IAsyncDisposable` for non-blocking asynchronous resource cleanup.
* **`PinnedBlockMemoryPool`**
   * *Description*: A memory pool that allocates page-pinned memory blocks for advanced native interop.
* **`IoQueue`**
   * *Description*: A high-performance task scheduling queue optimized for low-latency asynchronous IO completion.

### 3. Bitwise & Flags

* **`PackedBools8` / `PackedBools16` / `PackedBools32` / `PackedBools64`**
   * *Description*: Struct wrappers packing boolean flags into integer primitives (`byte`, `ushort`, `uint`, `ulong`).
   * *Key Feature*: Includes 1-cycle branchless range checks and hardware-accelerated set-bit counting via `BitOperations.PopCount`.
* **`Flags128` / `Flags256`**
   * *Description*: High-speed, fixed-size bitset structures backed by C# 12 `[InlineArray(N)]` attributes over `PackedBools64` chunks.
   * *Key Feature*: Division-free bitwise shifts for O(1) bit lookup across segment boundaries.

### 4. Extensions

* **`SpanByteReadExtensions`**
   * *Description*: Provides high-performance extension methods to read unmanaged types in big-endian and little-endian formats from byte spans, utilizing stackalloc reversions on endianness mismatch.
* **`ByteWriteExtensions`**
   * *Description*: Provides high-performance extension methods to write unmanaged types in big-endian and little-endian formats directly into byte spans and buffer writers.
* **`ServiceProviderExtensions`**
   * *Description*: Employs experimental C# 13 `extension` block syntax to resolve up to 8 generic services concurrently from an `IServiceProvider`.
* **`VoidResultExtensions`**
   * *Description*: Provides clean result mappings for void operations.

### 5. Results & Errors

* **`Result<TSuccess, TError>`**
   * *Description*: A premium, lightweight, IEquatable union struct representing successful or failed operation results.
   * *Key Feature*: Fully IEquatable, allocation-free deconstruction, and robust nullability protections to prevent string formatting crashes.
* **`VoidResult<TError>`**
   * *Description*: An allocation-free union struct representing operations returning void that may fail.
* **`Error` / `StringError`**
   * *Description*: Compact, readonly error-type wrappers designed for structured API responses.

### 6. Threading

* **`WorkPool`**
   * *Description*: A premium, high-performance, bounded asynchronous actor/work pool queue backed by `System.Threading.Channels`.
   * *Key Feature*: Fixed outer-task worker unwrapping concurrency bug to ensure reliable shutdown awaits.
* **`TaskExtensions`**
   * *Description*: Provides `WithAggregateException` task helpers to preserve and unwrap exceptions across concurrent worker queues.

### 7. Timing

* **`AsyncTimer`**
   * *Description*: A high-performance, stack-only `ref struct` timer designed for asynchronous operation performance tracking.
* **`StackTimer`**
   * *Description*: A lightweight, stack-only timer for synchronous block profiling.
