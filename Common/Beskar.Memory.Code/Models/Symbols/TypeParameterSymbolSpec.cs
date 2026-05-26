using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the characteristics and metadata of a type parameter symbol in a generic construct.
/// </summary>
public sealed record TypeParameterSymbolSpec
{
   /// <summary>
   /// Gets or sets the ordinal position of the type parameter in the type parameters list.
   /// </summary>
   public required int Ordinal { get; init; }
   private PackedBools8 _flags;

   private TypeParameterSymbolLoadFlags _loadedFlags;
   private ref TypeParameterSymbolLoadFlags LoadedFlags => ref _loadedFlags;
   
   /// <summary>
   /// Gets or sets a value indicating whether the type parameter allows ref-like types (e.g., <c>allows ref struct</c>).
   /// </summary>
   public bool AllowsRefLikeType
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type parameter has a constructor constraint (<c>new()</c>).
   /// </summary>
   public bool HasConstructorConstraint
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type parameter has a not-null constraint (<c>notnull</c>).
   /// </summary>
   public bool HasNotNullConstraint
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type parameter has a reference type constraint (<c>class</c>).
   /// </summary>
   public bool HasReferenceTypeConstraint
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type parameter has an unmanaged type constraint (<c>unmanaged</c>).
   /// </summary>
   public bool HasUnmanagedTypeConstraint
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the type parameter has a value type constraint (<c>struct</c>).
   /// </summary>
   public bool HasValueTypeConstraint
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }
   
   private SequenceArray<TypeSymbolArchetype>? _constraintTypes;

   /// <summary>
   /// Gets or sets the sequence of constraint types specified for the type parameter.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the constraint types are accessed before they are loaded or are unexpectedly null.</exception>
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

/// <summary>
/// Represents the lazy-loading state flags for type parameter symbol properties.
/// </summary>
public record struct TypeParameterSymbolLoadFlags
{
   private PackedBools8 Flags;

   /// <summary>
   /// Gets or sets a value indicating whether the constraint types have been loaded.
   /// </summary>
   public bool ConstraintTypes
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }
}
