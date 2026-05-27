using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

public sealed partial class SerializationGenerator
{
   private const string AttributeNameSpace = "Beskar.Memory.Serialization.Attributes";

   private const string AttributeIgnoreName = "BeskarIgnoreAttribute";
   private const string AttributeIgnoreFullName = $"{AttributeNameSpace}.{AttributeIgnoreName}";

   private const string AttributeObjectName = "BeskarObjectAttribute";
   private const string AttributeObjectFullName = $"{AttributeNameSpace}.{AttributeObjectName}";

   private const string AttributeOrderName = "BeskarOrderAttribute";
   private const string AttributeOrderFullName = $"{AttributeNameSpace}.{AttributeOrderName}";

   private const string AttributeUnionName = "BeskarUnionAttribute";
   private const string AttributeUnionFullName = $"{AttributeNameSpace}.{AttributeUnionName}";

   private const string AttributeUseSerializerName = "UseSerializerAttribute";
   private const string AttributeUseSerializerFullName = $"{AttributeNameSpace}.{AttributeUseSerializerName}";

   private static AttributeData? GetBeskarIgnoreAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsBeskarIgnoreAttribute);
   }

   private static bool IsBeskarIgnoreAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == AttributeIgnoreName 
         && IsRelevantAttribute(attribute);
   }

   private static AttributeData? GetBeskarObjectAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsBeskarObjectAttribute);
   }

   private static bool IsBeskarObjectAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == AttributeObjectName 
         && IsRelevantAttribute(attribute);
   }

   private static AttributeData? GetBeskarOrderAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsBeskarOrderAttribute);
   }

   private static bool IsBeskarOrderAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == AttributeOrderName 
         && IsRelevantAttribute(attribute);
   }

   private static AttributeData? GetBeskarUnionAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsBeskarUnionAttribute);
   }

   private static bool IsBeskarUnionAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == AttributeUnionName 
         && IsRelevantAttribute(attribute);
   }

   private static AttributeData? GetUseSerializerAttribute(ImmutableArray<AttributeData> attributes)
   {
      return attributes.FirstOrDefault(IsUseSerializerAttribute);
   }

   private static bool IsUseSerializerAttribute(AttributeData attribute)
   {
      return attribute.AttributeClass?.Name == AttributeUseSerializerName 
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
               Name: "Serialization",
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
         },
      };
   }
}
