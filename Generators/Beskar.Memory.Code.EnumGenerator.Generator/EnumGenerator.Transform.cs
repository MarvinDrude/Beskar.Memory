using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Code.EnumGenerator.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.EnumGenerator.Generator;

public sealed partial class EnumGenerator
{
   private static MaybeSpec<FastEnumSpec> Transform(
      GeneratorAttributeSyntaxContext context,
      CancellationToken ct)
   {
      ct.ThrowIfCancellationRequested();

      var symbol = (INamedTypeSymbol)context.TargetSymbol;
      var attributes = symbol.GetAttributes();

      if (GetFastEnumAttribute(attributes) is not { } attribute)
      {
         return DiagnosticBuilder<FastEnumSpec>.CreateEmpty();
      }

      ct.ThrowIfCancellationRequested();
      using var builder = DiagnosticBuilder<FastEnumSpec>.Create(8);

      var namedType = symbol.CreateNamedArchetype(CreateOptions());

      var displayNamesBuilder = System.Collections.Immutable.ImmutableArray.CreateBuilder<EnumFieldDisplayNameSpec>();
      
      var fields = namedType.NamedType.Fields;
      foreach (var fieldArchetype in fields.Array)
      {
         if (fieldArchetype.Symbol.Attributes.Array.FirstOrDefault(a => a is EnumDisplayAttributeSpec) is EnumDisplayAttributeSpec displaySpec)
         {
            if (displaySpec.Name is not null)
            {
               string? resourceCall = null;
               if (displaySpec.ResourceTypeFullName is not null)
               {
                  resourceCall = $"{displaySpec.ResourceTypeFullName}.{displaySpec.Name}";
               }

               displayNamesBuilder.Add(new EnumFieldDisplayNameSpec(
                  fieldArchetype.Symbol.Name,
                  resourceCall is null ? displaySpec.Name : null,
                  resourceCall));
            }
         }
      }

      return builder.Build(new FastEnumSpec(namedType, new Beskar.Memory.Collections.SequenceArray<EnumFieldDisplayNameSpec>(displayNamesBuilder.ToArray())));
   }

   private static ArchetypeTransformOptions CreateOptions()
   {
      var opts = new ArchetypeTransformOptions()
      {
         NamedTypes =
         {
            Depth = 2,
            FieldFilter = static (field) => field.HasConstantValue,
            Load = new NamedTypeSymbolLoadFlags()
            {
               Fields = true
            }
         },
         Symbols = new SymbolTransformOptions()
         {
            Load = new SymbolLoadFlags()
            {
               Attributes = true
            }
         }
      };

      opts.RegisterAttribute(
         "global::System.ComponentModel.DataAnnotations.DisplayAttribute",
         (symbol, attributeData) =>
         {
            string? name = null;
            string? resourceTypeFullName = null;

            var nameArg = attributeData.NamedArguments.FirstOrDefault(kv => kv.Key == "Name");
            if (nameArg.Value.Value is string n)
            {
               name = n;
            }

            var resourceTypeArg = attributeData.NamedArguments.FirstOrDefault(kv => kv.Key == "ResourceType");
            if (resourceTypeArg.Value.Value is INamedTypeSymbol resourceType)
            {
               resourceTypeFullName = resourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }

            return new EnumDisplayAttributeSpec(name, resourceTypeFullName);
         });

      return opts;
   }
}
