namespace Beskar.Memory.Code.TypeIdGenerator.Attributes;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class TypeSafeIdAttribute(
   bool isOverrideString = true,
   bool addImplicitConversions = true,
   bool addExplicitConversions = true,
   bool isSpanParsable = true,
   bool addJsonConverter = true)
   : Attribute
{
   public bool IsOverrideString { get; init; } = isOverrideString;
   
   public bool AddImplicitConversions { get; init; } = addImplicitConversions;
   
   public bool AddExplicitConversions { get; init; } = addExplicitConversions;
   
   public bool IsSpanParsable { get; init; } = isSpanParsable;
   
   public bool AddJsonConverter { get; init; } = addJsonConverter;
}