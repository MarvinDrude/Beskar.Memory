using System;
using Beskar.Memory.Serialization.Interfaces;
using Beskar.Memory.Serialization.Serializers.Collections;

namespace Beskar.Memory.Serialization.Resolvers;

/// <summary>
/// A resolver that handles array types (1D, 2D, 4D).
/// </summary>
public class ArraySerializerResolver : ISerializerResolver
{
   public bool TryResolve<T>() where T : allows ref struct
   {
      var type = typeof(T);
      if (!type.IsArray)
      {
         return false;
      }

      var elementType = type.GetElementType();
      if (elementType == null)
      {
         return false;
      }

      var rank = type.GetArrayRank();
      var openSerializerType = rank switch
      {
         1 => typeof(ArraySerializer<>),
         2 => typeof(Array2DSerializer<>),
         4 => typeof(Array4DSerializer<>),
         _ => null
      };

      if (openSerializerType == null)
      {
         return false;
      }

      try
      {
         var closedSerializerType = openSerializerType.MakeGenericType(elementType);
         SerializerRegistry.Register(type, closedSerializerType);
         return true;
      }
      catch
      {
         return false;
      }
   }
}
