using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
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
      foreach (var member in symbol.GetMembers())
      {
         if (member is IFieldSymbol { HasConstantValue: true } field)
         {
            var displayAttr = field.GetAttributes().FirstOrDefault(a =>
               a.AttributeClass?.ToDisplayString() == "System.ComponentModel.DataAnnotations.DisplayAttribute");

            if (displayAttr is not null)
            {
               string? nameValue = null;
               string? resourceCall = null;

               var nameArg = displayAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "Name");
               if (nameArg.Value.Value is string name)
               {
                  nameValue = name;
               }

               var resourceTypeArg = displayAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "ResourceType");
               if (resourceTypeArg.Value.Value is INamedTypeSymbol resourceType)
               {
                  var resourceTypeFullName = resourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                  if (nameValue is not null)
                  {
                     resourceCall = $"{resourceTypeFullName}.{nameValue}";
                  }
               }

               if (nameValue is not null)
               {
                  displayNamesBuilder.Add(new EnumFieldDisplayNameSpec(
                     field.Name,
                     resourceCall is null ? nameValue : null,
                     resourceCall));
               }
            }
         }
      }

      return builder.Build(new FastEnumSpec(namedType, displayNamesBuilder));
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
         }
      };

      return opts;
   }
}
