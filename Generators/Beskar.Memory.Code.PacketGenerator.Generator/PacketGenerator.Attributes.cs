using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.PacketGenerator.Generator;

public sealed partial class PacketGenerator
{
   private const string AttributeNameSpace = "Beskar.Memory.Code.PacketGenerator.Attributes";
   
   private const string PacketAttributeName = "PacketAttribute";
   private const string PacketAttributeFullName = $"{AttributeNameSpace}.{PacketAttributeName}";
   
   private const string PacketRegistryAttributeName = "PacketRegistryAttribute";
   private const string PacketRegistryAttributeFullName = $"{AttributeNameSpace}.{PacketRegistryAttributeName}";
   private const string PacketRegistryGenericAttributeFullName = $"{PacketRegistryAttributeFullName}`1";
   
   private static AttributeData? GetPacketRegistryAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsPacketRegistryAttribute);
   }
   
   private static AttributeData? GetPacketAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsPacketAttribute);
   }
   
   private static bool IsPacketRegistryAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == PacketRegistryAttributeName
         && IsRelevantAttribute(attribute);
   }
   
   private static bool IsPacketAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == PacketAttributeName 
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
               Name: "PacketGenerator",
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
   
   private static bool IsInCommonNamespace(ISymbol symbol)
   {
      return symbol is
      {
         ContainingNamespace:
         {
            Name: "Common",
            ContainingNamespace:
            {
               Name: "PacketGenerator",
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
