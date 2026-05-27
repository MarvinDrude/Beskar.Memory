namespace Beskar.Memory.Serialization.Attributes;

/// <summary>
/// Specifies a custom serializer to be used for this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class UseSerializerAttribute(Type serializerType) : Attribute
{
   public Type SerializerType { get; } = serializerType;
}

