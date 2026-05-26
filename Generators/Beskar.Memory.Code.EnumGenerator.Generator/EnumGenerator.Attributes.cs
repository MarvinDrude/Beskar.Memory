using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.EnumGenerator.Generator;

public sealed partial class EnumGenerator
{
   private const string AttributeNameSpace = "Beskar.Memory.Code.EnumGenerator.Attributes";
   
   private const string FastEnumAttributeName = "FastEnumAttribute";
   private const string FastEnumAttributeFullName = $"{AttributeNameSpace}.{FastEnumAttributeName}";

   private static AttributeData? GetFastEnumAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsFastEnumAttribute);
   }
   
   private static bool IsFastEnumAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == FastEnumAttributeName 
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
               Name: "EnumGenerator",
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
