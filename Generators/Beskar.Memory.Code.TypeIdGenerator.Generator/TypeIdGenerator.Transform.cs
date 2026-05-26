using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Code.TypeIdGenerator.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.TypeIdGenerator.Generator;

public sealed partial class TypeIdGenerator
{
   private static MaybeSpec<TypeSafeIdSpec> Transform(
      GeneratorAttributeSyntaxContext context,
      CancellationToken ct)
   {
      ct.ThrowIfCancellationRequested();

      var symbol = (INamedTypeSymbol)context.TargetSymbol;
      var attributes = symbol.GetAttributes();

      if (GetTypeSafeIdAttribute(attributes) is not { } attribute)
      {
         return DiagnosticBuilder<TypeSafeIdSpec>.CreateEmpty();
      }
      
      ct.ThrowIfCancellationRequested();
      
      using var builder = DiagnosticBuilder<TypeSafeIdSpec>.Create(8);
      var attributeSpec = GetAttributeSpec(attribute);

      var namedInfo = symbol.CreateNamedArchetype(CreateTransformOptions());
      ct.ThrowIfCancellationRequested();

      if (namedInfo.NamedType.Methods.Array is not [var constructor])
      {
         return builder.Add(InvalidTargetDiagnosticId).Build();
      }
      
      if (constructor.Method.Parameters.Array is not [var parameter])
      {
         return builder.Add(InvalidTargetDiagnosticId).Build();
      }

      if (!string.Equals(parameter.Symbol.Name, "Value", StringComparison.OrdinalIgnoreCase))
      {
         return builder.Add(InvalidTargetDiagnosticId).Build();
      }
      
      return builder.Build(new TypeSafeIdSpec(attributeSpec, namedInfo));
   }
   
   private static TypeSafeIdAttributeSpec GetAttributeSpec(AttributeData data)
   {
      return new TypeSafeIdAttributeSpec(
         data.DetermineBoolValue("IsOverrideString", 0),
         data.DetermineBoolValue("AddImplicitConversions", 1),
         data.DetermineBoolValue("AddExplicitConversions", 2),
         data.DetermineBoolValue("IsSpanParsable", 3),
         data.DetermineBoolValue("AddJsonConverter", 4));
   }

   private static ArchetypeTransformOptions CreateTransformOptions()
   {
      var options = new ArchetypeTransformOptions
      {
         NamedTypes =
         {
            MethodFilter = static (method) => 
               method.MethodKind is MethodKind.Constructor 
                  && method.Parameters.Length == 1
                  && method.Parameters[0].Name.Equals("Value", StringComparison.OrdinalIgnoreCase),
            Load = new NamedTypeSymbolLoadFlags()
            {
               Methods = true,
            }
         },
         Methods = new MethodTransformOptions()
         {
            Depth = 2,
            Load = new MethodSymbolLoadFlags()
            {
               Parameters = true,
            }
         },
         Parameters = new ParameterTransformOptions()
         {
            Depth = 3,
            Load = new ParameterSymbolLoadFlags()
            {
               Type = true,
            }
         }
      };

      return options;
   }
}