using Beskar.Memory.Code.Interfaces.Specs;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator.Models;

public readonly record struct TypeSafeIdAttributeSpec(
   bool IsOverrideString,
   bool AddImplicitConversions,
   bool AddExplicitConversions,
   bool IsSpanParsable,
   bool AddJsonConverter)
   : IAttributeSpec
{
   public bool Equals(IAttributeSpec? other)
   {
      return other is TypeSafeIdAttributeSpec spec && Equals(spec);
   }
}