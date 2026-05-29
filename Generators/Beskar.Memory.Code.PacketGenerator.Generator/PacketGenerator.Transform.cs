using Beskar.Memory.Code.Common;
using Beskar.Memory.Code.Common.Symbols;
using Beskar.Memory.Code.Diagnostics;
using Beskar.Memory.Code.Models.Diagnostics;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.PacketGenerator.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.PacketGenerator.Generator;

public sealed partial class PacketGenerator
{
   private static MaybeSpec<PacketSpec> Transform(
      GeneratorAttributeSyntaxContext context,
      CancellationToken ct)
   {
      ct.ThrowIfCancellationRequested();

      var symbol = (INamedTypeSymbol)context.TargetSymbol;
      var attributes = symbol.GetAttributes();

      if (GetPacketAttribute(attributes) is not { } attribute)
      {
         return DiagnosticBuilder<PacketSpec>.CreateEmpty();
      }
      
      ct.ThrowIfCancellationRequested();
      using var builder = DiagnosticBuilder<PacketSpec>.Create(8);

      ITypeSymbol actualPacketSymbol = symbol;
      var wrapperArg = attribute.NamedArguments.FirstOrDefault(x => x.Key == "Wrapper");
      if (wrapperArg.Value.Value is INamedTypeSymbol wrapperOpenGeneric)
      {
         actualPacketSymbol = wrapperOpenGeneric.OriginalDefinition.Construct(symbol);
      }

      if (actualPacketSymbol is not INamedTypeSymbol namedPacketSymbol)
      {
         return builder.Add(InvalidTargetDiagnosticId).Build();
      }

      var namedType = namedPacketSymbol.CreateNamedArchetype(CreateTransformOptions());

      if (namedType.Type.AllInterfaces.Array.FirstOrDefault(
            x => x.Symbol is
            {
               Name: "IPacket", 
               NameSpace: "Beskar.Memory.Code.PacketGenerator.Interfaces"
            })
          is not { Symbol.Name: "IPacket" })
      {
         return builder.Add(InvalidTargetDiagnosticId).Build();
      }
      
      var types = attribute.DetermineTypeArrayValues("Types", 0);
      
      return builder.Build(new PacketSpec(
         [.. types.Select(x => x?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty)], 
         namedType));
   }

   private static MaybeSpec<PacketRegistrySpec> TransformRegistry(
      GeneratorAttributeSyntaxContext context,
      CancellationToken ct)
   {
      ct.ThrowIfCancellationRequested();

      var symbol = (INamedTypeSymbol)context.TargetSymbol;
      var attributes = symbol.GetAttributes();

      if (GetPacketRegistryAttribute(attributes) is not { } attribute)
      {
         return DiagnosticBuilder<PacketRegistrySpec>.CreateEmpty();
      }

      var stateType = attribute.AttributeClass is { IsGenericType: true, TypeArguments: [var typeArg] }
         ? typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
         : null;
      
      ct.ThrowIfCancellationRequested();
      using var builder = DiagnosticBuilder<PacketRegistrySpec>.Create(8);
      var namedType = symbol.CreateNamedArchetype(CreateTransformRegistryOptions());

      if (!HasRegistryBaseType(symbol))
      {
         return builder.Add(InvalidRegistryTargetDiagnosticId).Build();
      }
      
      return builder.Build(new PacketRegistrySpec(namedType, stateType));
   }

   private static bool HasRegistryBaseType(INamedTypeSymbol symbol)
   {
      while (true)
      {
         if (symbol.Name is "BasePacketRegistry" && IsInCommonNamespace(symbol))
         {
            return true;
         }

         if (symbol.BaseType is { SpecialType: SpecialType.System_Object } or null)
         {
            return false;
         }

         symbol = symbol.BaseType;
      }
   }

   private static ArchetypeTransformOptions CreateTransformOptions()
   {
      var options = new ArchetypeTransformOptions
      {
         Types =
         {
            Load = new TypeSymbolLoadFlags()
            {
               AllInterfaces = true,
            }
         }
      };

      return options;
   }
   
   private static ArchetypeTransformOptions CreateTransformRegistryOptions()
   {
      var options = new ArchetypeTransformOptions
      {
         NamedTypes =
         {
            Depth = 3,
            MethodFilter = static (method) => method.MethodKind is MethodKind.Constructor && method.Parameters.Length > 0,
            Load = new NamedTypeSymbolLoadFlags()
            {
               Methods = true
            }
         },
         Types =
         {
            Depth = 3,
            Load = new TypeSymbolLoadFlags()
            {
               BaseType = true
            }
         },
         Methods =
         {
            Depth = 3,
            Load = new MethodSymbolLoadFlags()
            {
               Parameters = true
            }
         },
         Parameters =
         {
            Depth = 12,
            Load = new ParameterSymbolLoadFlags()
            {
               Type = true
            }
         }
      };

      return options;
   }
}
