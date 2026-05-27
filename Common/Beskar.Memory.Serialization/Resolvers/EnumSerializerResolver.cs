using System;
using Beskar.Memory.Serialization.Interfaces;
using Beskar.Memory.Serialization.Serializers.System;

namespace Beskar.Memory.Serialization.Resolvers;

/// <summary>
/// A resolver that handles enum types automatically using EnumSerializer.
/// </summary>
public class EnumSerializerResolver : ISerializerResolver
{
   public bool TryResolve<T>() where T : allows ref struct
   {
      var type = typeof(T);
      if (!type.IsEnum)
      {
         return false;
      }

      try
      {
         var closedSerializerType = typeof(EnumSerializer<>).MakeGenericType(type);
         SerializerRegistry.Register(type, closedSerializerType);
         return true;
      }
      catch
      {
         return false;
      }
   }
}
