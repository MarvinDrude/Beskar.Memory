using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

public sealed record FieldSymbolSpec
{
   public required RefKind RefKind { get; init; }

   private PackedBools8 _flags;
   
   private FieldSymbolLoadFlags _loadedFlags;
   private ref FieldSymbolLoadFlags LoadedFlags => ref _loadedFlags;
   
   public bool HasConstantValue
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   public bool IsConst
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   public bool IsRequired
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }
   
   public bool IsVolatile
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }
   
   public bool IsReadOnly
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   private TypeSymbolArchetype? _type;
   public TypeSymbolArchetype Type
   {
      get => LoadedFlags.Type 
         ? _type ?? throw new InvalidOperationException("Type should be loaded but is null.") 
         : throw new InvalidOperationException("Type is not loaded.");
      set
      {
         _type = value;
         LoadedFlags.Type = true;
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

public record struct FieldSymbolLoadFlags
{
   private PackedBools8 Flags;
   
   public bool Type
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }

   public bool Attributes
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }
}

