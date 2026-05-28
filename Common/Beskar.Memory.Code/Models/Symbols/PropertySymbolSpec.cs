using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the characteristics and metadata of a property symbol.
/// </summary>
public sealed record PropertySymbolSpec
{
   /// <summary>
   /// Gets or sets the reference kind of the property.
   /// </summary>
   public required RefKind RefKind { get; init; }
   private PackedBools8 _flags;
   
   private PropertySymbolLoadFlags _loadedFlags;
   private ref PropertySymbolLoadFlags LoadedFlags => ref _loadedFlags;

   public bool IsTypeLoaded => _loadedFlags.Type;
   
   /// <summary>
   /// Gets or sets a value indicating whether the property has a getter accessor.
   /// </summary>
   public bool HasGetter
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the property has a setter accessor.
   /// </summary>
   public bool HasSetter
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the property is an indexer.
   /// </summary>
   public bool IsIndexer
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the property is read-only.
   /// </summary>
   public bool IsReadOnly
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the property is required.
   /// </summary>
   public bool IsRequired
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   private TypeSymbolArchetype? _type;

   /// <summary>
   /// Gets or sets the type archetype of the property.
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

   /// <summary>
   /// Gets or sets the getter method archetype of the property, if loaded.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the getter is accessed before it is loaded or is unexpectedly null.</exception>
   public MethodSymbolArchetype? Getter
   {
      get => LoadedFlags.Getter 
         ? field ?? throw new InvalidOperationException("Getter should be loaded but is null.") 
         : throw new InvalidOperationException("Getter is not loaded.");
      set
      {
         field = value;
         LoadedFlags.Getter = true;
      }
   }

   /// <summary>
   /// Gets or sets the setter method archetype of the property, if loaded.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the setter is accessed before it is loaded or is unexpectedly null.</exception>
   public MethodSymbolArchetype? Setter
   {
      get => LoadedFlags.Setter 
         ? field ?? throw new InvalidOperationException("Setter should be loaded but is null.") 
         : throw new InvalidOperationException("Setter is not loaded.");
      set
      {
         field = value;
         LoadedFlags.Setter = true;
      }
   }
   
   private SequenceArray<IAttributeSpec>? _attributes;

   /// <summary>
   /// Gets or sets the sequence of attributes applied to the property.
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
/// Represents the lazy-loading state flags for property symbol properties.
/// </summary>
public record struct PropertySymbolLoadFlags
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
   /// Gets or sets a value indicating whether the setter method archetype has been loaded.
   /// </summary>
   public bool Setter
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the getter method archetype has been loaded.
   /// </summary>
   public bool Getter
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
