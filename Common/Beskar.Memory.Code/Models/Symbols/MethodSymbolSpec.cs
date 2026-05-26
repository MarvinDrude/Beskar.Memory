using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the characteristics and metadata of a method symbol.
/// </summary>
[DebuggerDisplay("Method-Kind: {MethodKind, nq}")]
public sealed record MethodSymbolSpec
{
   /// <summary>
   /// Gets or sets the kind of the method (e.g., ordinary, constructor, property accessor).
   /// </summary>
   public required MethodKind MethodKind { get; init; }
   private PackedBools8 _flags;
   
   private MethodSymbolLoadFlags _loadedFlags;
   private ref MethodSymbolLoadFlags LoadedFlags => ref _loadedFlags;
   
   /// <summary>
   /// Gets or sets a value indicating whether the method has a void return type.
   /// </summary>
   public bool HasVoidReturn
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the method returns by reference (<c>ref</c>).
   /// </summary>
   public bool ReturnsByRef
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the method returns by read-only reference (<c>ref readonly</c>).
   /// </summary>
   public bool ReturnsByRefReadonly
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the method is read-only.
   /// </summary>
   public bool IsReadOnly
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the method is an iterator.
   /// </summary>
   public bool IsIterator
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the method is asynchronous (<c>async</c>).
   /// </summary>
   public bool IsAsync
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }

   /// <summary>
   /// Gets or sets the return type archetype of the method, if loaded.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the return type is accessed before it is loaded.</exception>
   public TypeSymbolArchetype? ReturnType
   {
      get => LoadedFlags.ReturnType 
         ? field 
         : throw new InvalidOperationException("Return type is not loaded.");
      set
      {
         field = value;
         LoadedFlags.ReturnType = true;
      }
   }

   private SequenceArray<ParameterSymbolArchetype>? _parameters;

   /// <summary>
   /// Gets or sets the sequence of parameters for the method.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the parameters are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<ParameterSymbolArchetype> Parameters
   {
      get => LoadedFlags.Parameters 
         ? _parameters ?? throw new InvalidOperationException("Parameters should be loaded but is null.") 
         : throw new InvalidOperationException("Parameters are not loaded.");
      set
      {
         _parameters = value;
         LoadedFlags.Parameters = true;
      }
   }
   
   private SequenceArray<IAttributeSpec>? _attributes;

   /// <summary>
   /// Gets or sets the sequence of attributes applied to the method.
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
/// Represents the lazy-loading state flags for method symbol properties.
/// </summary>
public record struct MethodSymbolLoadFlags
{
   private PackedBools8 Flags;
   
   /// <summary>
   /// Gets or sets a value indicating whether the return type has been loaded.
   /// </summary>
   public bool ReturnType
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the parameters have been loaded.
   /// </summary>
   public bool Parameters
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the attributes have been loaded.
   /// </summary>
   public bool Attributes
   {
      get => Flags.Get(2);
      set => Flags.Set(2, value);
   }
}
