using System;
using System.Buffers;
using System.Collections.Generic;

namespace Beskar.Memory.Serialization;

/// <summary>
/// Provides a lightweight, allocation-free context for tracking object references during serialization.
/// </summary>
public struct SerializationContext : IDisposable
{
   [ThreadStatic]
   private static SerializationContext _current;
   public static ref SerializationContext Current => ref _current;

   private object?[]? _references;
   private int[]? _ids;
   private Dictionary<object, int>? _dict;
   private int _count;

   public bool TryGetReferenceId(object obj, out int referenceId)
   {
      if (_dict != null)
      {
         return _dict.TryGetValue(obj, out referenceId);
      }

      if (_references == null || _ids == null)
      {
         referenceId = 0;
         return false;
      }

      for (var i = 0; i < _count; i++)
      {
         if (ReferenceEquals(_references[i], obj))
         {
            referenceId = _ids[i];
            return true;
         }
      }

      referenceId = 0;
      return false;
   }

   public int Register(object obj)
   {
      if (_dict != null)
      {
         var id = _count + 1;
         _dict[obj] = id;
         _count++;

         return id;
      }

      if (_references == null || _ids == null)
      {
         _references = ArrayPool<object?>.Shared.Rent(16);
         _ids = ArrayPool<int>.Shared.Rent(16);
         _count = 0;
      }

      if (_count >= 16)
      {
         _dict = new Dictionary<object, int>(16, ReferenceEqualityComparer.Instance);
         for (var i = 0; i < _count; i++)
         {
            _dict[_references[i]!] = _ids[i];
         }

         ArrayPool<object?>.Shared.Return(_references, clearArray: true);
         ArrayPool<int>.Shared.Return(_ids);

         _references = null;
         _ids = null;

         var id = _count + 1;
         _dict[obj] = id;
         _count++;

         return id;
      }

      var newId = _count + 1;
      _references[_count] = obj;
      _ids[_count] = newId;

      _count++;
      return newId;
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
