namespace Beskar.Memory.Code.PacketGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PacketRegistryAttribute<TState> : Attribute;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PacketRegistryAttribute : Attribute;
