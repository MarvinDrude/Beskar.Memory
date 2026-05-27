using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Attributes;

/// <summary>
/// Automatically creates a <see cref="ISerializer{T}"/> for the decorated type.
/// And registers it in the <see cref="SerializerRegistry{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class BeskarObjectAttribute : Attribute;
