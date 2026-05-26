using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Symbols;

/// <summary>
/// Provides extension members for <see cref="ITypeSymbol"/> to construct archetype models, perform namespace checks, and analyze type attributes.
/// </summary>
public static class TypeSymbolExtensions
{
   extension<TSymbol>(TSymbol type)
      where TSymbol : ITypeSymbol
   {
      /// <summary>
      /// Creates a <see cref="TypeSymbolArchetype"/> from the type symbol using the specified options.
      /// </summary>
      /// <param name="options">The options to configure archetype transformation.</param>
      /// <returns>A constructed <see cref="TypeSymbolArchetype"/> representing the type symbol.</returns>
      public TypeSymbolArchetype CreateArchetype(ArchetypeTransformOptions? options = null)
      {
         options ??= new ArchetypeTransformOptions();
         options.ClearCache();
         
         return TypeSymbolArchetypeTransformer.Transform(type, options: options);
      }
      
      /// <summary>
      /// Retrieves all members of the type, including inherited members from base types.
      /// </summary>
      /// <returns>An enumerable collection of all <see cref="ISymbol"/> members.</returns>
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

      /// <summary>
      /// Gets a value indicating whether the type represents a non-generic <see cref="Task"/>.
      /// </summary>
      public bool IsVoidTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 0, Name: "Task" };

      /// <summary>
      /// Gets a value indicating whether the type represents a generic <see cref="Task{TResult}"/>.
      /// </summary>
      public bool IsGenericTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 1, Name: "Task" };

      /// <summary>
      /// Gets a value indicating whether the type represents a non-generic <see cref="ValueTask"/>.
      /// </summary>
      public bool IsVoidValueTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 0, Name: "ValueTask" };

      /// <summary>
      /// Gets a value indicating whether the type represents a generic <see cref="ValueTask{TResult}"/>.
      /// </summary>
      public bool IsGenericValueTask => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Arity: 1, Name: "ValueTask" };

      /// <summary>
      /// Gets a value indicating whether the type represents a task-like type (<see cref="Task"/> or <see cref="ValueTask"/>).
      /// </summary>
      public bool IsAnyTaskType => type.IsInSystemTasksNamespace 
         && type is INamedTypeSymbol { Name: "Task" or "ValueTask" };

      /// <summary>
      /// Gets a value indicating whether the type resides within the <c>System.Threading.Tasks</c> namespace.
      /// </summary>
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
      
      /// <summary>
      /// Gets a value indicating whether the type resides within the <c>System</c> namespace.
      /// </summary>
      public bool IsInSystemNamespace => type is
      {
         ContainingNamespace:
         {
            Name: "System",
            ContainingNamespace.IsGlobalNamespace: true
         }
      };
      
      /// <summary>
      /// Gets a string representation of the C# declaration keywords for the type kind (e.g., "struct", "class", "record struct").
      /// </summary>
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
      
      /// <summary>
      /// Gets a value indicating whether the type is a struct.
      /// </summary>
      public bool IsStruct => type.TypeKind == TypeKind.Struct;

      /// <summary>
      /// Gets a value indicating whether the type is an enum.
      /// </summary>
      public bool IsEnum => type.TypeKind == TypeKind.Enum;

      /// <summary>
      /// Gets a value indicating whether the type is a delegate.
      /// </summary>
      public bool IsDelegate => type.TypeKind == TypeKind.Delegate;

      /// <summary>
      /// Gets a value indicating whether the type is an interface.
      /// </summary>
      public bool IsInterface => type.TypeKind == TypeKind.Interface;

      /// <summary>
      /// Gets a value indicating whether the type is a record.
      /// </summary>
      public bool IsRecord => type.IsRecord;

      /// <summary>
      /// Gets a value indicating whether the type is a class.
      /// </summary>
      public bool IsClass => type.TypeKind == TypeKind.Class;

      /// <summary>
      /// Gets a value indicating whether the type is an integer-based numeric type.
      /// </summary>
      public bool IsNumber => type.SpecialType.IsNumber;

      /// <summary>
      /// Gets a value indicating whether the type is a floating-point or decimal numeric type.
      /// </summary>
      public bool IsFloatingNumber => type.SpecialType.IsFloatingNumber;

      /// <summary>
      /// Gets a value indicating whether the type is any numeric type.
      /// </summary>
      public bool IsAnyNumber => type.SpecialType.IsAnyNumber;
      
      /// <summary>
      /// Gets a value indicating whether the type is <see cref="string"/>.
      /// </summary>
      public bool IsString => type.SpecialType == SpecialType.System_String;

      /// <summary>
      /// Gets a value indicating whether the type is <see cref="bool"/>.
      /// </summary>
      public bool IsBoolean => type.SpecialType == SpecialType.System_Boolean;

      /// <summary>
      /// Gets a value indicating whether the type is <see cref="char"/>.
      /// </summary>
      public bool IsChar => type.SpecialType == SpecialType.System_Char;

      /// <summary>
      /// Gets a value indicating whether the type is <see cref="object"/>.
      /// </summary>
      public bool IsObject => type.SpecialType == SpecialType.System_Object;
      
      /// <summary>
      /// Gets a value indicating whether the type represents a <see cref="Guid"/>.
      /// </summary>
      public bool IsGuid => type.Name == "Guid" && type.IsInSystemNamespace;
   }
}
