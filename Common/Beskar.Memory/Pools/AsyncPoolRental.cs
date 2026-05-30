using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Beskar.Memory.Pools;

/// <summary>
/// A stack-allocated, zero-allocation <see langword="ref struct"/> rental wrapper for an async-disposable pooled object.
/// Automatically returns the object to the pool asynchronously upon disposal.
/// </summary>
/// <typeparam name="T">The type of pooled objects.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly ref struct AsyncPoolRental<T> : IAsyncDisposable
   where T : class, IAsyncDisposable
{
   private readonly AsyncDisposableObjectPool<T> _pool;
   
   /// <summary>
   /// Gets the rented object.
   /// </summary>
   public T Value { get; }

   /// <summary>
   /// Initializes a new instance of the <see cref="AsyncPoolRental{T}"/> struct.
   /// </summary>
   /// <param name="pool">The pool to return the object to.</param>
   /// <param name="value">The rented object.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AsyncPoolRental(AsyncDisposableObjectPool<T> pool, T value)
   {
      _pool = pool;
      Value = value;
   }

   /// <summary>
   /// Returns the rented object to the pool asynchronously.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTask DisposeAsync()
   {
      if (_pool is not null && Value is not null)
      {
         var vt = _pool.ReturnAsync(Value);
         return vt.IsCompletedSuccessfully ? ValueTask.CompletedTask : Await(vt);
      }
      return ValueTask.CompletedTask;

      static async ValueTask Await(ValueTask<bool> vt)
      {
         await vt;
      }
   }
}
