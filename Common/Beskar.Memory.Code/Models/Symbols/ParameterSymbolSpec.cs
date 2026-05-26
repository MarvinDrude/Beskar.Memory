using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the characteristics and metadata of a parameter symbol.
/// </summary>
public sealed record ParameterSymbolSpec
{
   /// <summary>
   /// Gets or sets the ordinal position of the parameter in the parameters list.
   /// </summary>
   public required int Ordinal { get; init; }

   /// <summary>
   /// Gets or sets the scope kind of the parameter (e.g., scoped parameter).
   /// </summary>
   public required ScopedKind ScopeKind { get; init; }

   /// <summary>
   /// Gets or sets the reference kind of the parameter (e.g., ref, out, in).
   /// </summary>
   public required RefKind RefKind { get; init; }
   
   private PackedBools8 _flags;
   
   private ParameterSymbolLoadFlags _loadedFlags;
   private ref ParameterSymbolLoadFlags LoadedFlags => ref _loadedFlags;
   
   /// <summary>
   /// Gets or sets a value indicating whether the parameter has an explicit default value.
   /// </summary>
   public bool HasExplicitDefaultValue
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the parameter is a <c>params</c> array.
   /// </summary>
   public bool IsParamsArray
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the parameter is a <c>params</c> collection.
   /// </summary>
   public bool IsParamsCollection
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the parameter is a discard.
   /// </summary>
   public bool IsDiscard
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the parameter is optional.
   /// </summary>
   public bool IsOptional
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }

   private TypeSymbolArchetype? _type;

   /// <summary>
   /// Gets or sets the type archetype of the parameter.
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
   /// Gets or sets the sequence of attributes applied to the parameter.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the attributes are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<IAttributeSpec> Attributes
   {
      get => LoadedFlags.Attributes 
         ? _attributes ?? throw new InvalidOperationException("Attributes should be loaded but is null.") 
         : throw new InvalidOperationException("Attributes are not loaded.");
      set
      {
         _attributes = value;
         LoadedFlags.Attributes = true;
      }
   }
}

/// <summary>
/// Represents the lazy-loading state flags for parameter symbol properties.
/// </summary>
public record struct ParameterSymbolLoadFlags
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
