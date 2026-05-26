using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Models.Symbols;

public sealed record TypeParameterSymbolSpec
{
   public required int Ordinal { get; init; }
   private PackedBools8 _flags;

   private TypeParameterSymbolLoadFlags _loadedFlags;
   private ref TypeParameterSymbolLoadFlags LoadedFlags => ref _loadedFlags;
   
   public bool AllowsRefLikeType
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   public bool HasConstructorConstraint
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   public bool HasNotNullConstraint
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }
   
   public bool HasReferenceTypeConstraint
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }
   
   public bool HasUnmanagedTypeConstraint
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   public bool HasValueTypeConstraint
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }
   
   private SequenceArray<TypeSymbolArchetype>? _constraintTypes;
   public SequenceArray<TypeSymbolArchetype> ConstraintTypes
   {
      get => LoadedFlags.ConstraintTypes 
         ? _constraintTypes ?? throw new InvalidOperationException("Constraint types should be loaded but is null.")
         : throw new InvalidOperationException("Constraint types are not loaded.");
      set
      {
         _constraintTypes = value;
         LoadedFlags.ConstraintTypes = true;
      }
   }
}

public record struct TypeParameterSymbolLoadFlags
{
   private PackedBools8 Flags;

   public bool ConstraintTypes
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }
}

