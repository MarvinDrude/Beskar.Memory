using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Resolvers;

/// <summary>
/// A resolver that checks if a serializer is already present in the registry.
/// </summary>
public class RegisteredSerializerResolver : ISerializerResolver
{
   public bool TryResolve<T>() where T : allows ref struct
   {
      if (SerializerRegistry<T>.IsAlreadyRegistered)
      {
         return true;
      }

      if (SerializerRegistry.TryGetExplicitSerializer(typeof(T), out var serializerType))
      {
         SerializerRegistry.Register(typeof(T), serializerType);
         return true;
      }

      return false;
   }
}
