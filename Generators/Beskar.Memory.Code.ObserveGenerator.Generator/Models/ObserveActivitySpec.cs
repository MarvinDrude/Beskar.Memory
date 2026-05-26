using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Interfaces.Specs;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator.Models;

public sealed record ObserveActivitySpec : IAttributeSpec
{
   public required string Name { get; init; }
   
   public required string Version { get; init; }
   
   public bool Equals(IAttributeSpec? other)
   {
      return other is ObserveActivitySpec spec && Equals(spec);
   }

   public static ObserveActivitySpec Create(ISymbol symbol, AttributeData attribute)
   {
      return new ObserveActivitySpec()
      {
         Name = attribute.DetermineStringValue("Name", 0) ?? symbol.Name,
         Version = attribute.DetermineStringValue("Version", 1) ?? "1.0.0"
      };
   }
}
