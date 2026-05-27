namespace Beskar.Memory.Serialization.Attributes;

/// <summary>
/// Attribute to mark a union type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
public sealed class BeskarUnionAttribute(int tag, Type type) : Attribute
{
   /// <summary>
   /// The tag value of the union.
   /// </summary>
   public int Tag { get; } = tag;

   /// <summary>
   /// The type of the union.
   /// </summary>
   public Type Type { get; } = type;
}
