using System;

namespace Beskar.Memory.Pools;

/// <summary>
/// Configurations and delegate hooks for constructing an <see cref="ObjectPool{T}"/>.
/// </summary>
/// <typeparam name="T">The type of objects pooled.</typeparam>
public sealed class ObjectPoolOptions<T>
   where T : class
{
   /// <summary>
   /// Gets the factory delegate used to instantiate new pooled instances.
   /// </summary>
   public required Func<T> FactoryFunc { get; init; }

   /// <summary>
   /// Gets the optional delegate called when an item is returned, determining if it can be accepted back into the pool.
   /// Defaults to a delegate returning <see langword="true"/>.
   /// </summary>
   public Func<T, bool> ReturnFunc { get; init; } = _ => true;

   /// <summary>
   /// Gets the maximum capacity of the pool.
   /// Defaults to 1024.
   /// </summary>
   public int MaxSize { get; init; } = 1024;

   /// <summary>
   /// Gets the initial size of the pool, populated upon construction.
   /// Defaults to 0.
   /// </summary>
   public int InitialSize { get; init; } = 0;
}
