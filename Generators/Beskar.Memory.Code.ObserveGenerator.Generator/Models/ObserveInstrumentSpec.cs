using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Interfaces.Specs;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator.Models;

public sealed record ObserveInstrumentSpec : IAttributeSpec
{
   public required string PropertyName { get; init; }
   
   public required string InstrumentFullName { get; init; }
   
   public required string TypeFullName { get; init; }
   
   public required string Name { get; init; }
   
   public string? Unit { get; init; }
   
   public string? Description { get; init; }
   
   public bool Equals(IAttributeSpec? other)
   {
      return other is ObserveInstrumentSpec spec && Equals(spec);
   }

   public static ObserveInstrumentSpec Create(ISymbol symbol, AttributeData attribute)
   {
      var propertyName = attribute.DetermineStringValue("PropertyName", 0) ?? "Unknown";
      
      return new ObserveInstrumentSpec()
      {
         PropertyName = propertyName,
         InstrumentFullName = attribute.DetermineEnumFullName("Kind", 1) ?? "Beskar.Memory.Code.ObserveGenerator.Enums.InstrumentKind.Counter",
         TypeFullName = attribute.DetermineTypeValue("Type", 2)?.ToDisplayString() ?? "int",
         Name = attribute.DetermineStringValue("Name", 3) ?? propertyName,
         Unit = attribute.DetermineStringValue("Unit", 4),
         Description = attribute.DetermineStringValue("Description", 5)
      };
   }
}
