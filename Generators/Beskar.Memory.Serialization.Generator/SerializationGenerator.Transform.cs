using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Code.TypeIdGenerator.Generator.Models;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

public sealed partial class SerializationGenerator
{
   private static MaybeSpec<SerializeSpec> Transform(
      GeneratorAttributeSyntaxContext context,
      CancellationToken ct)
   {
      ct.ThrowIfCancellationRequested();

      var symbol = (INamedTypeSymbol)context.TargetSymbol;
      var attributes = symbol.GetAttributes();

      if (GetBeskarObjectAttribute(attributes) is not { } attribute)
      {
         return DiagnosticBuilder<SerializeSpec>.CreateEmpty();
      }

      ct.ThrowIfCancellationRequested();
      using var builder = DiagnosticBuilder<SerializeSpec>.Create(8);

      var syntaxReferences = symbol.DeclaringSyntaxReferences;
      var isPartial = false;

      foreach (var r in syntaxReferences)
      {
         var node = r.GetSyntax(ct);

         if (node is not Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax typeDecl) continue;
         if (!typeDecl.Modifiers.Any(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)) continue;

         isPartial = true;
         break;
      }

      if (!isPartial)
      {
         return builder.Add(InvalidTargetDiagnosticId).Build();
      }

      var namedType = symbol.CreateNamedArchetype(CreateTransformOptions());
      ct.ThrowIfCancellationRequested();

      var implementsSerializationCallback = namedType.Type.AllInterfaces.Array.Any(x =>
         x.Symbol is { Name: "ISerializationCallback", NameSpace: "Beskar.Memory.Serialization.Interfaces" });

      var unionSpecsList = new List<UnionSpec>();
      foreach (var attr in attributes.Where(IsBeskarUnionAttribute))
      {
         if (attr.ConstructorArguments is [{ Value: int tag }, { Value: INamedTypeSymbol childTypeSymbol }])
         {
            var childArchetype = childTypeSymbol.CreateNamedArchetype(CreateTransformOptions());
            unionSpecsList.Add(new UnionSpec(tag, childArchetype));
         }
      }

      var membersList = new List<MemberSpec>();
      var orders = new HashSet<int>();
      var duplicateOrders = new HashSet<int>();
      var hasMissingOrder = false;

      foreach (var memberSymbol in symbol.GetMembers())
      {
         if (memberSymbol.IsStatic) continue;

         var memberAttrs = memberSymbol.GetAttributes();
         if (GetBeskarIgnoreAttribute(memberAttrs) is not null) continue;

         if (memberSymbol.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal))
         {
            continue;
         }

         bool isProperty;
         if (memberSymbol is IPropertySymbol)
         {
            isProperty = true;
         }
         else if (memberSymbol is IFieldSymbol)
         {
            isProperty = false;
         }
         else
         {
            continue;
         }

         var orderAttr = GetBeskarOrderAttribute(memberAttrs);
         if (orderAttr is null)
         {
            builder.Add(MissingOrderAttributeRule.Id, memberSymbol.Name, symbol.Name);
            hasMissingOrder = true;
            continue;
         }

         var order = 0;
         if (orderAttr.ConstructorArguments.Length > 0 && orderAttr.ConstructorArguments[0].Value is int orderVal)
         {
            order = orderVal;
         }

         if (!orders.Add(order))
         {
            builder.Add(DuplicateOrderRule.Id, symbol.Name, order.ToString(), memberSymbol.Name);
            duplicateOrders.Add(order);
         }

         var useSerializerAttr = GetUseSerializerAttribute(memberAttrs);
         NamedTypeSymbolArchetype? serializerType = null;
         if (useSerializerAttr is not null && useSerializerAttr.ConstructorArguments.Length > 0 &&
             useSerializerAttr.ConstructorArguments[0].Value is INamedTypeSymbol customSerializerSymbol)
         {
            serializerType = customSerializerSymbol.CreateNamedArchetype(CreateTransformOptions());
         }

         membersList.Add(new MemberSpec(memberSymbol.Name, isProperty, order, serializerType));
      }

      if (hasMissingOrder || duplicateOrders.Count > 0)
      {
         return builder.Build();
      }

      var sortedMembers = membersList.OrderBy(x => x.Order).ToList();

      // Check constructor matching
      var isUseConstructor = false;
      var constructorParameterIndices = new List<int>();
      var constructors = symbol.InstanceConstructors;

      IMethodSymbol? bestConstructor = null;
      var bestConstructorParamIndices = new List<int>();

      foreach (var ctor in constructors)
      {
         if (ctor.Parameters.Length == 0) continue;

         var paramIndices = new List<int>();
         var matchedAll = true;

         foreach (var param in ctor.Parameters)
         {
            var matchedIndex = sortedMembers.FindIndex(m => m.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
            if (matchedIndex >= 0)
            {
               paramIndices.Add(matchedIndex);
            }
            else
            {
               matchedAll = false;
               break;
            }
         }

         if (matchedAll && paramIndices.Count == ctor.Parameters.Length)
         {
            bestConstructor = ctor;
            bestConstructorParamIndices = paramIndices;
            break;
         }
      }

      if (bestConstructor is not null)
      {
         isUseConstructor = true;
         constructorParameterIndices = bestConstructorParamIndices;
      }
      else
      {
         var hasParameterless = constructors.Any(c => c.Parameters.Length == 0);
         if (!hasParameterless && !symbol.IsValueType && sortedMembers.Count > 0)
         {
            return builder.Add(MissingConstructorRule.Id, symbol.Name).Build();
         }
      }

      var isOpenGeneric = symbol.Arity > 0;

      var spec = new SerializeSpec(
         namedType,
         isOpenGeneric,
         isUseConstructor,
         new SequenceArray<MemberSpec>([.. sortedMembers]),
         new SequenceArray<UnionSpec>([.. unionSpecsList]),
         new SequenceArray<int>([.. constructorParameterIndices]),
         implementsSerializationCallback
      );

      return builder.Build(spec);
   }

   private static ArchetypeTransformOptions CreateTransformOptions()
   {
      var options = new ArchetypeTransformOptions
      {
         NamedTypes = new NamedTypeTransformOptions
         {
            Depth = 4,
            Load = new NamedTypeSymbolLoadFlags()
            {
               Methods = true,
               Properties = true,
               Fields = true,
               TypeParameters = true,
               TypeArguments = true,
               Attributes = true
            }
         },
         Methods = new MethodTransformOptions
         {
            Depth = 4,
            Load = new MethodSymbolLoadFlags()
            {
               Parameters = true
            }
         },
         Properties = new PropertyTransformOptions
         {
            Depth = 4,
            Load = new PropertySymbolLoadFlags()
            {
               Type = true,
               Attributes = true
            }
         },
         Fields = new FieldTransformOptions
         {
            Depth = 4,
            Load = new FieldSymbolLoadFlags()
            {
               Type = true,
               Attributes = true
            }
         },
         Parameters = new ParameterTransformOptions
         {
            Depth = 4,
            Load = new ParameterSymbolLoadFlags()
            {
               Type = true
            }
         },
         Types = new TypeTransformOptions
         {
            Depth = 4,
            Load = new TypeSymbolLoadFlags()
            {
               BaseType = true,
               AllInterfaces = true
            }
         }
      };

      return options;
   }
}
