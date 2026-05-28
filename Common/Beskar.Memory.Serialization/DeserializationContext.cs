using System;
using System.Buffers;
using System.Collections.Generic;

namespace Beskar.Memory.Serialization;

/// <summary>
/// Provides a lightweight, allocation-free context for tracking object references during deserialization.
/// </summary>
public struct DeserializationContext(BeSerializerOptions? options) : IDisposable
{
   [ThreadStatic]
   private static DeserializationContext _current;
   public static ref DeserializationContext Current => ref _current;

   private object?[]? _references = null;
   private int[]? _ids = null;
   private Dictionary<int, object>? _dict = null;

   private int _count = 0;
   private int _depth = 0;

   private BeSerializerOptions? _options = options;

   /// <summary>
   /// Gets the active deserialization options.
   /// </summary>
   public BeSerializerOptions Options => _options ??= BeSerializerOptions.Default;

   /// <summary>
   /// Increments recursion depth and checks against configured limits.
   /// </summary>
   public void IncrementDepth()
   {
      _depth++;
      if (_depth > Options.MaxDepth)
      {
         throw new InvalidOperationException($"Max depth of {Options.MaxDepth} exceeded during deserialization, preventing stack overflow.");
      }
   }

   /// <summary>
   /// Decrements recursion depth.
   /// </summary>
   public void DecrementDepth()
   {
      _depth--;
   }

   public void Register(int id, object obj)
   {
      if (_dict != null)
      {
         _dict[id] = obj;
         _count++;
         return;
      }

      if (_references == null || _ids == null)
      {
         _references = ArrayPool<object?>.Shared.Rent(16);
         _ids = ArrayPool<int>.Shared.Rent(16);
         _count = 0;
      }

      if (_count >= 16)
      {
         _dict = new Dictionary<int, object>(16);
         for (var i = 0; i < _count; i++)
         {
            _dict[_ids[i]] = _references[i]!;
         }

         ArrayPool<object?>.Shared.Return(_references, clearArray: true);
         ArrayPool<int>.Shared.Return(_ids);

         _references = null;
         _ids = null;

         _dict[id] = obj;
         _count++;
         return;
      }

      _references[_count] = obj;
      _ids[_count] = id;
      _count++;
   }

   public object GetByReferenceId(int referenceId)
   {
      if (_dict != null)
      {
         if (_dict.TryGetValue(referenceId, out var obj))
         {
            return obj;
         }
      }
      else if (_references != null && _ids != null)
      {
         for (var i = 0; i < _count; i++)
         {
            if (_ids[i] == referenceId)
            {
               return _references[i]!;
            }
         }
      }

      throw new InvalidOperationException($"Reference ID {referenceId} not found in context.");
   }

   public void Dispose()
   {
      if (_references != null)
      {
         ArrayPool<object?>.Shared.Return(_references, clearArray: true);
         _references = null;
      }

      if (_ids != null)
      {
         ArrayPool<int>.Shared.Return(_ids);
         _ids = null;
      }

      _dict = null;
      _count = 0;
      _depth = 0;
   }
}
