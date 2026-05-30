using Beskar.Memory.Code.Interfaces.Specs;

namespace Beskar.Memory.Code.EnumGenerator.Generator.Models;

public sealed record EnumDisplayAttributeSpec(
   string? Name,
   string? ResourceTypeFullName) : IAttributeSpec
{
   public bool Equals(IAttributeSpec? other)
   {
      return other is EnumDisplayAttributeSpec spec &&
             Name == spec.Name &&
             ResourceTypeFullName == spec.ResourceTypeFullName;
   }
}
