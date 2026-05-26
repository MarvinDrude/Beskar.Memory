namespace Beskar.Memory.Code.ObserveGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ObserveActivityAttribute(
   string? name = null,
   string? version = null)
   : Attribute
{
   public string? Name { get; init; } = name;
   
   public string? Version { get; init; } = version;
}
