using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

public sealed partial class TypeIdGenerator
{
   private const string AttributeNameSpace = "Beskar.Memory.Code.TypeIdGenerator.Attributes";

   private const string AttributeTypeIdName = "TypeSafeIdAttribute";
   private const string AttributeTypeIdFullName = $"{AttributeNameSpace}.{AttributeTypeIdName}";

   private static AttributeData? GetTypeSafeIdAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsTypeSafeIdAttribute);
   }

   private static bool IsTypeSafeIdAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == AttributeTypeIdName 
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
               Name: "Marker",
               ContainingNamespace:
               {
                  Name: "TypeIdGenerator",
                  ContainingNamespace:
                  {
                     Name: "CodeGeneration",
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