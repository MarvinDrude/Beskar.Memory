using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the characteristics and metadata of a field symbol.
/// </summary>
public sealed record FieldSymbolSpec
{
   /// <summary>
   /// Gets or sets the reference kind of the field.
   /// </summary>
   public required RefKind RefKind { get; init; }

   private PackedBools8 _flags;
   
   private FieldSymbolLoadFlags _loadedFlags;
   private ref FieldSymbolLoadFlags LoadedFlags => ref _loadedFlags;
   
   /// <summary>
   /// Gets or sets a value indicating whether the field has a constant value.
   /// </summary>
   public bool HasConstantValue
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the field is constant (<c>const</c>).
   /// </summary>
   public bool IsConst
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the field is required.
   /// </summary>
   public bool IsRequired
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the field is volatile.
   /// </summary>
   public bool IsVolatile
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the field is read-only.
   /// </summary>
   public bool IsReadOnly
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   private TypeSymbolArchetype? _type;

   /// <summary>
   /// Gets or sets the type archetype of the field.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the type is accessed before it is loaded or is unexpectedly null.</exception>
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

   /// <summary>
   /// Gets or sets the sequence of attributes applied to the field.
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
/// Represents the lazy-loading state flags for field symbol properties.
/// </summary>
public record struct FieldSymbolLoadFlags
{
   private PackedBools8 Flags;
   
   /// <summary>
   /// Gets or sets a value indicating whether the type archetype has been loaded.
   /// </summary>
   public bool Type
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the attributes have been loaded.
   /// </summary>
   public bool Attributes
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }
}
