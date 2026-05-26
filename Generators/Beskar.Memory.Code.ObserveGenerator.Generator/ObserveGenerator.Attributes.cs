using System.Collections.Immutable;
using Beskar.Memory.Code.ObserveGenerator.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.ObserveGenerator.Generator;

public sealed partial class ObserveGenerator
{
   private const string AttributeNameSpace = "Beskar.Memory.Code.ObserveGenerator.Attributes";
   
   private const string ActivityAttributeName = "ObserveActivityAttribute";
   private const string ActivityAttributeFullName = $"{AttributeNameSpace}.{ActivityAttributeName}";
   
   private const string MeterAttributeName = "ObserveMeterAttribute";
   private const string MeterAttributeFullName = $"{AttributeNameSpace}.{MeterAttributeName}";
   
   private const string InstrumentAttributeName = "ObserveInstrumentAttribute";
   private const string InstrumentAttributeFullName = $"{AttributeNameSpace}.{InstrumentAttributeName}";
   
   private const string ObserveAttributeName = "ObserveAttribute";
   private const string ObserveAttributeFullName = $"{AttributeNameSpace}.{ObserveAttributeName}";

   private static ObserveMeterSpec? GetMeterAttribute(ISymbol symbol, ImmutableArray<AttributeData> attributes)
   {
      return attributes.Where(IsMeterAttribute)
         .Select(x => ObserveMeterSpec.Create(symbol, x))
         .FirstOrDefault();
   }
   
   private static ObserveInstrumentSpec[] GetInstrumentAttributes(ISymbol symbol, ImmutableArray<AttributeData> attributes)
   {
      return attributes.Where(IsInstrumentAttribute)
         .Select(x => ObserveInstrumentSpec.Create(symbol, x))
         .ToArray();
   }
   
   private static ObserveActivitySpec? GetActivityAttribute(ISymbol symbol, ImmutableArray<AttributeData> attributes)
   {
      return attributes.Where(IsActivityAttribute)
         .Select(x => ObserveActivitySpec.Create(symbol, x))
         .FirstOrDefault();
   }
   
   private static bool IsMeterAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == MeterAttributeName 
         && IsRelevantAttribute(attribute);
   }
   
   private static bool IsInstrumentAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == InstrumentAttributeName 
         && IsRelevantAttribute(attribute);
   }
   
   private static bool IsActivityAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == ActivityAttributeName 
         && IsRelevantAttribute(attribute);
   }
   
   private static bool IsRelevantAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass is
      {
         ContainingNamespace:
         {
            Name: "Attributes",
            ContainingNamespace:
            {
               Name: "ObserveGenerator",
               ContainingNamespace:
               {
                  Name: "Code",
                  ContainingNamespace:
                  {
                     Name: "Memory",
                     ContainingNamespace:
                     {
                        Name: "Beskar",
                        ContainingNamespace.IsGlobalNamespace: true
                     }
                  }
               }
            }
         },
      };
   }
}
