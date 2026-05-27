using System;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Resolvers;

/// <summary>
/// A resolver that matches closed generic types against registered open generic serializers.
/// </summary>
public class OpenGenericSerializerResolver : ISerializerResolver
{
   public bool TryResolve<T>() where T : allows ref struct
   {
      var type = typeof(T);
      if (!type.IsGenericType)
      {
         return false;
      }

      var openGenericType = type.GetGenericTypeDefinition();
      if (SerializerRegistry.TryGetOpenGenericMapping(
             openGenericType, out var openGenericSerializerType))
      {
         try
         {
            var typeArguments = type.GetGenericArguments();
            var closedSerializerType = openGenericSerializerType.MakeGenericType(typeArguments);

            SerializerRegistry.Register(type, closedSerializerType);

            return true;
         }
         catch
         {
            return false; // Constraint mismatch or instantiation failure
         }
      }

      return false;
   }
}
