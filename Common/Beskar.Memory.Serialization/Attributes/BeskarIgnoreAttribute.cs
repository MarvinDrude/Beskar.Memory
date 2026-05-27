namespace Beskar.Memory.Serialization.Attributes;

/// <summary>
/// Ignore this property or field during serialization.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BeskarIgnoreAttribute : Attribute;
