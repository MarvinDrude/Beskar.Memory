using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

public static class TypeSymbolExtensions
{
   extension<TSymbol>(TSymbol type)
      where TSymbol : ITypeSymbol
   {
      public TypeSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return TypeSymbolArchetypeTransformer.Transform(type, options: options);
      }
      
      public IEnumerable<ISymbol> GetAllMembers()
      {
         ITypeSymbol? currentType = type;
         while (currentType is not null)
         {
            foreach (var member in currentType.GetMembers())
            {
               yield return member;
            }
            
            currentType = currentType.BaseType;
         }
      }

      public bool IsVoidTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 0, Name: "Task" };
      public bool IsGenericTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 1, Name: "Task" };
      public bool IsVoidValueTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 0, Name: "ValueTask" };
      public bool IsGenericValueTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 1, Name: "ValueTask" };
      public bool IsAnyTaskType => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Name: "Task" or "ValueTask" };

      public bool IsInSystemTasksNamespace => type is
      {
         ContainingNamespace:
         {
            Name: "Tasks",
            ContainingNamespace:
            {
               Name: "Threading",
               ContainingNamespace:
               {
                  Name: "System",
                  ContainingNamespace.IsGlobalNamespace: true
               }
            }
         }
      };
      
      public bool IsInSystemNamespace => type is
      {
         ContainingNamespace:
         {
            Name: "System",
            ContainingNamespace.IsGlobalNamespace: true
         }
      };
      
      public string TypeAsString => type switch
      {
         { IsRecord: true, TypeKind: TypeKind.Struct } => "record struct",
         { IsRecord: true } => "record",
         { TypeKind: TypeKind.Interface } => "interface",
         { TypeKind: TypeKind.Struct } => "struct",
         { TypeKind: TypeKind.Enum } => "enum",
         { TypeKind: TypeKind.Delegate } => "delegate",
         
         _ => "class"
      };
      
      public bool IsStruct => type.TypeKind == TypeKind.Struct;
      public bool IsEnum => type.TypeKind == TypeKind.Enum;
      public bool IsDelegate => type.TypeKind == TypeKind.Delegate;
      public bool IsInterface => type.TypeKind == TypeKind.Interface;
      public bool IsRecord => type.IsRecord;
      public bool IsClass => type.TypeKind == TypeKind.Class;

      public bool IsNumber => type.SpecialType.IsNumber;
      public bool IsFloatingNumber => type.SpecialType.IsFloatingNumber;
      public bool IsAnyNumber => type.SpecialType.IsAnyNumber;
      
      public bool IsString => type.SpecialType == SpecialType.System_String;
      public bool IsBoolean => type.SpecialType == SpecialType.System_Boolean;
      public bool IsChar => type.SpecialType == SpecialType.System_Char;
      public bool IsObject => type.SpecialType == SpecialType.System_Object;
      
      public bool IsGuid => type.Name == "Guid" && type.IsInSystemNamespace;
   }
}

