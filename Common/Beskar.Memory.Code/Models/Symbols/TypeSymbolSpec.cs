using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

[DebuggerDisplay("Type-Kind: {Kind, nq}")]
public sealed record TypeSymbolSpec
{
   public required TypeKind Kind { get; init; }
   public required SpecialType SpecialType { get; init; }
   public required NullableAnnotation NullableAnnotation { get; init; }
   
   private PackedBools8 _flags;
   
   private TypeSymbolLoadFlags _loadedFlags;
   private ref TypeSymbolLoadFlags LoadedFlags => ref _loadedFlags;

   public bool HasBaseType
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }

   public bool IsReadOnly
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   public bool IsRecord
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }

   public bool IsReferenceType
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }
   
   public bool IsRefLikeType
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }

   public bool IsTupleType
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }
   
   public bool IsUnmanagedType
   {
      get => _flags.Get(6);
      set => _flags.Set(6, value);
   }
   
   public bool IsValueType
   {
      get => _flags.Get(7);
      set => _flags.Set(7, value);
   }
   
   private SequenceArray<NamedTypeSymbolArchetype>? _allInterfaces;
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

public record struct TypeSymbolLoadFlags
{
   private PackedBools8 Flags;

   public bool Interfaces
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }
   
   public bool BaseType
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }
   
   public bool AllInterfaces
   {
      get => Flags.Get(2);
      set => Flags.Set(2, value);
   }
   
   public bool Attributes
   {
      get => Flags.Get(3);
      set => Flags.Set(3, value);
   }
}

