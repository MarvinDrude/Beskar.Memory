using System;
using System.Buffers;
using System.Collections.Generic;

namespace Beskar.Memory.Serialization;

/// <summary>
/// Provides a lightweight, allocation-free context for tracking object references during serialization.
/// </summary>
public struct SerializationContext() : IDisposable
{
   [ThreadStatic]
   private static SerializationContext _current;
   public static ref SerializationContext Current => ref _current;

   private object?[]? _references = ArrayPool<object?>.Shared.Rent(16);
   private int[]? _ids = ArrayPool<int>.Shared.Rent(16);

   private int _count = 0;

   public bool TryGetReferenceId(object obj, out int referenceId)
   {
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
      if (_references == null || _ids == null)
      {
         _references = ArrayPool<object?>.Shared.Rent(16);
         _ids = ArrayPool<int>.Shared.Rent(16);
      }

      if (_count == _references.Length)
      {
         var newReferences = ArrayPool<object?>.Shared.Rent(_count * 2);
         var newIds = ArrayPool<int>.Shared.Rent(_count * 2);

         Array.Copy(_references, newReferences, _count);
         Array.Copy(_ids, newIds, _count);

         ArrayPool<object?>.Shared.Return(_references, clearArray: true);
         ArrayPool<int>.Shared.Return(_ids);

         _references = newReferences;
         _ids = newIds;
      }

      var id = _count + 1;

      _references[_count] = obj;
      _ids[_count] = id;

      _count++;
      return id;
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

      _count = 0;
   }
}

/// <summary>
/// Provides a lightweight, allocation-free context for tracking object references during deserialization.
/// </summary>
public struct DeserializationContext() : IDisposable
{
   [ThreadStatic]
   private static DeserializationContext _current;
   public static ref DeserializationContext Current => ref _current;

   private object?[]? _references = ArrayPool<object?>.Shared.Rent(16);
   private int[]? _ids = ArrayPool<int>.Shared.Rent(16);

   private int _count = 0;

   public void Register(int id, object obj)
   {
      if (_references == null || _ids == null)
      {
         _references = ArrayPool<object?>.Shared.Rent(16);
         _ids = ArrayPool<int>.Shared.Rent(16);
      }

      if (_count == _references.Length)
      {
         var newReferences = ArrayPool<object?>.Shared.Rent(_count * 2);
         var newIds = ArrayPool<int>.Shared.Rent(_count * 2);

         Array.Copy(_references, newReferences, _count);
         Array.Copy(_ids, newIds, _count);

         ArrayPool<object?>.Shared.Return(_references, clearArray: true);
         ArrayPool<int>.Shared.Return(_ids);

         _references = newReferences;
         _ids = newIds;
      }

      _references[_count] = obj;
      _ids[_count] = id;
      _count++;
   }

   public object GetByReferenceId(int referenceId)
   {
      if (_references != null && _ids != null)
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

      _count = 0;
   }
}
