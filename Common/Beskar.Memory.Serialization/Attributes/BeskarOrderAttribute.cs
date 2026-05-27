namespace Beskar.Memory.Serialization.Attributes;

/// <summary>
/// Determines the order in which properties / fields are serialized.
/// Can never have duplicate values.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BeskarOrderAttribute(int order)
   : Attribute
{
   public int Order { get; } = order;
}
