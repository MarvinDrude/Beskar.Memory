using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the characteristics and metadata of a type symbol.
/// </summary>
[DebuggerDisplay("Type-Kind: {Kind, nq}")]
public sealed record TypeSymbolSpec
{
   /// <summary>
   /// Gets or sets the kind of the type (e.g., class, struct, interface, array, etc.).
   /// </summary>
   public required TypeKind Kind { get; init; }

   /// <summary>
   /// Gets or sets the special type of the type (e.g., System.Int32, System.String, or None).
   /// </summary>
   public required SpecialType SpecialType { get; init; }

   /// <summary>
   /// Gets or sets the nullable annotation applied to the type.
   /// </summary>
   public required NullableAnnotation NullableAnnotation { get; init; }
   
   private PackedBools8 _flags;
   
   private TypeSymbolLoadFlags _loadedFlags;
   private ref TypeSymbolLoadFlags LoadedFlags => ref _loadedFlags;

   /// <summary>
   /// Gets or sets a value indicating whether the type has a base type.
   /// </summary>
   public bool HasBaseType
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the type is read-only.
   /// </summary>
   public bool IsReadOnly
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type is a record (<c>record class</c> or <c>record struct</c>).
   /// </summary>
   public bool IsRecord
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the type is a reference type.
   /// </summary>
   public bool IsReferenceType
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type is a ref-like type (<c>ref struct</c>).
   /// </summary>
   public bool IsRefLikeType
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the type is a tuple type.
   /// </summary>
   public bool IsTupleType
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type is an unmanaged type.
   /// </summary>
   public bool IsUnmanagedType
   {
      get => _flags.Get(6);
      set => _flags.Set(6, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type is a value type.
   /// </summary>
   public bool IsValueType
   {
      get => _flags.Get(7);
      set => _flags.Set(7, value);
   }
   
   private SequenceArray<NamedTypeSymbolArchetype>? _allInterfaces;

   /// <summary>
   /// Gets or sets the sequence of all interfaces directly or indirectly implemented by this type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the interfaces are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<NamedTypeSymbolArchetype> AllInterfaces
   {
      get => LoadedFlags.AllInterfaces 
         ? _allInterfaces ?? throw new InvalidOperationException("AllInterfaces should be loaded but is null.") 
         : throw new InvalidOperationException("AllInterfaces is not loaded.");
      set
      {
         _allInterfaces = value;
         LoadedFlags.AllInterfaces = true;
      }
   }
   
   private SequenceArray<NamedTypeSymbolArchetype>? _interfaces;

   /// <summary>
   /// Gets or sets the sequence of interfaces directly implemented by this type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the interfaces are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<NamedTypeSymbolArchetype> Interfaces
   {
      get => LoadedFlags.Interfaces 
         ? _interfaces ?? throw new InvalidOperationException("Interfaces should be loaded but is null.") 
         : throw new InvalidOperationException("Interfaces is not loaded.");
      set
      {
         _interfaces = value;
         LoadedFlags.Interfaces = true;
      }
   }

   /// <summary>
   /// Gets or sets the base type archetype of this type, if loaded.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the base type is accessed before it is loaded.</exception>
   public NamedTypeSymbolArchetype? BaseType
   {
      get => LoadedFlags.BaseType 
         ? field 
         : throw new InvalidOperationException("BaseType is not loaded.");
      set
      {
         field = value;
         LoadedFlags.BaseType = true;
      }
   }
   
   private SequenceArray<IAttributeSpec>? _attributes;

   /// <summary>
   /// Gets or sets the sequence of attributes applied to the type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the attributes are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<IAttributeSpec> Attributes
   {
      get => LoadedFlags.Attributes 
         ? _attributes ?? throw new InvalidOperationException("Attributes should be loaded but is null.") 
         : throw new InvalidOperationException("Attributes is not loaded.");
      set
      {
         _attributes = value;
         LoadedFlags.Attributes = true;
      }
   }
}

/// <summary>
/// Represents the lazy-loading state flags for type symbol properties.
/// </summary>
public record struct TypeSymbolLoadFlags
{
   private PackedBools8 Flags;

   /// <summary>
   /// Gets or sets a value indicating whether the interfaces have been loaded.
   /// </summary>
   public bool Interfaces
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the base type has been loaded.
   /// </summary>
   public bool BaseType
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether all implemented interfaces have been loaded.
   /// </summary>
   public bool AllInterfaces
   {
      get => Flags.Get(2);
      set => Flags.Set(2, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the attributes have been loaded.
   /// </summary>
   public bool Attributes
   {
      get => Flags.Get(3);
      set => Flags.Set(3, value);
   }
}
