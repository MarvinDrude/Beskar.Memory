namespace Beskar.Memory.Serialization.Interfaces;

/// <summary>
/// Defines a contract for a resolver that can dynamically resolve and register serializers for a type.
/// </summary>
public interface ISerializerResolver
{
   /// <summary>
   /// Tries to resolve and register a serializer for the specified type <typeparamref name="T"/>.
   /// </summary>
   /// <typeparam name="T">The type to resolve a serializer for.</typeparam>
   /// <returns>True if a serializer was successfully resolved and registered; otherwise, false.</returns>
   bool TryResolve<T>() where T : allows ref struct;
}
