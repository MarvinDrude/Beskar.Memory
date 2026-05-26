using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Interfaces.Specs;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator.Models;

public sealed record ObserveMeterSpec : IAttributeSpec
{
   public required string Name { get; init; }
   
   public required string Version { get; init; }
   
   public bool Equals(IAttributeSpec? other)
   {
      return other is ObserveMeterSpec spec && Equals(spec);
   }

   public static ObserveMeterSpec Create(ISymbol symbol, AttributeData attribute)
   {
      return new ObserveMeterSpec()
      {
         Name = attribute.DetermineStringValue("Name", 0) ?? symbol.Name,
         Version = attribute.DetermineStringValue("Version", 1) ?? "1.0.0"
      };
   }
}
