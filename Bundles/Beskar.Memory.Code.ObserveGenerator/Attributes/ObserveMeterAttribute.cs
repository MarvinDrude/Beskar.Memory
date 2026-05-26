namespace Beskar.Memory.Code.ObserveGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ObserveMeterAttribute(
   string? name = null,
   string? version = null)
   : Attribute
{
   public string? Name { get; init; } = name;

   public string? Version { get; init; } = version;
}
