using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the general characteristics and metadata of a symbol.
/// </summary>
[DebuggerDisplay("{FullName, nq}")]
public sealed record SymbolSpec
{
   /// <summary>
   /// Gets or sets the name of the symbol.
   /// </summary>
   public required string Name { get; init; }

   /// <summary>
   /// Gets or sets the metadata name of the symbol.
   /// </summary>
   public required string MetadataName { get; init; }

   /// <summary>
   /// Gets or sets the full name of the symbol.
   /// </summary>
   public required string FullName { get; init; }
   
   /// <summary>
   /// Gets or sets the namespace containing the symbol, or <c>null</c> if there isn't one.
   /// </summary>
   public string? NameSpace { get; init; }
   
   /// <summary>
   /// Gets or sets the kind of the symbol.
   /// </summary>
   public required SymbolKind Kind { get; init; }

   /// <summary>
   /// Gets or sets the declared accessibility of the symbol.
   /// </summary>
   public required Accessibility Accessibility { get; init; }
   
   private PackedBools8 _flags;
   
   private SymbolLoadFlags _loadedFlags;
   private ref SymbolLoadFlags LoadedFlags => ref _loadedFlags;
 
   /// <summary>
   /// Gets or sets a value indicating whether the symbol is static.
   /// </summary>
   public bool IsStatic
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the symbol is abstract.
   /// </summary>
   public bool IsAbstract
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the symbol is virtual.
   /// </summary>
   public bool IsVirtual
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the symbol is sealed.
   /// </summary>
   public bool IsSealed
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the symbol is an override.
   /// </summary>
   public bool IsOverride
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the symbol is implicitly declared.
   /// </summary>
   public bool IsImplicitlyDeclared
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the symbol is an extern.
   /// </summary>
   public bool IsExtern
   {
      get => _flags.Get(6);
      set => _flags.Set(6, value);
   }
   
   private SequenceArray<IAttributeSpec>? _attributes;

   /// <summary>
   /// Gets or sets the sequence of attributes applied to the symbol.
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
/// Represents the lazy-loading state flags for general symbol properties.
/// </summary>
public record struct SymbolLoadFlags
{
   private PackedBools8 Flags;
   
   /// <summary>
   /// Gets or sets a value indicating whether the attributes have been loaded.
   /// </summary>
   public bool Attributes
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }
}
