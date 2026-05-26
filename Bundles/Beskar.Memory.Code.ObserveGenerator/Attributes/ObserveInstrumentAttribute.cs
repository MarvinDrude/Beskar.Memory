using Beskar.Memory.Code.ObserveGenerator.Enums;

namespace Beskar.Memory.Code.ObserveGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ObserveInstrumentAttribute(
   string propertyName,
   InstrumentKind kind,
   Type type,
   string? name = null,
   string? unit = null,
   string? description = null)
   : Attribute
{
   public string PropertyName { get; init; } = propertyName;
   
   public InstrumentKind Kind { get; init; } = kind;
   
   public Type Type { get; init; } = type;
   
   public string? Name { get; init; } = name;
   
   public string? Unit { get; init; } = unit;
   
   public string? Description { get; init; } = description;
}
