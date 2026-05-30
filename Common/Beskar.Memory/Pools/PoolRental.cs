using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Beskar.Memory.Pools;

/// <summary>
/// A stack-allocated, zero-allocation <see langword="ref struct"/> rental wrapper for a pooled object.
/// Automatically returns the object to the pool upon disposal.
/// </summary>
/// <typeparam name="T">The type of pooled objects.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly ref struct PoolRental<T> : IDisposable
   where T : class
{
   private readonly ObjectPool<T> _pool;
   
   /// <summary>
   /// Gets the rented object.
   /// </summary>
   public T Value { get; }

   /// <summary>
   /// Initializes a new instance of the <see cref="PoolRental{T}"/> struct.
   /// </summary>
   /// <param name="pool">The pool to return the object to.</param>
   /// <param name="value">The rented object.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public PoolRental(ObjectPool<T> pool, T value)
   {
      _pool = pool;
      Value = value;
   }

   /// <summary>
   /// Returns the rented object to the pool.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Dispose()
   {
      if (_pool is not null && Value is not null)
      {
         _pool.Return(Value);
      }
   }
}
