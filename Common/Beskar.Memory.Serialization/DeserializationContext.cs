using System;
using System.Buffers;
using System.Collections.Generic;

namespace Beskar.Memory.Serialization;

/// <summary>
/// Provides a lightweight, allocation-free context for tracking object references during deserialization.
/// </summary>
public struct DeserializationContext : IDisposable
{
   [ThreadStatic]
   private static DeserializationContext _current;
   public static ref DeserializationContext Current => ref _current;

   private object?[]? _references;
   private int[]? _ids;
   private Dictionary<int, object>? _dict;
   private int _count;

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
   }
}
