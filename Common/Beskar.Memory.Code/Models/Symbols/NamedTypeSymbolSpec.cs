using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

/// <summary>
/// Specifies the characteristics and metadata of a named type symbol (e.g., class, struct, interface, enum).
/// </summary>
[DebuggerDisplay("NamedType, Arity: {Arity, nq}")]
public sealed record NamedTypeSymbolSpec
{
   /// <summary>
   /// Gets or sets the arity (number of generic type parameters) of the named type.
   /// </summary>
   public required int Arity { get; init; }
   private PackedBools8 _flags;
   
   private NamedTypeSymbolLoadFlags _loadedFlags;
   private ref NamedTypeSymbolLoadFlags LoadedFlags => ref _loadedFlags;

   public bool ArePropertiesLoaded => _loadedFlags.Properties;
   
   /// <summary>
   /// Gets or sets a value indicating whether the type is file-local.
   /// </summary>
   public bool IsFileLocal
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the type is an enum.
   /// </summary>
   public bool IsEnum
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   private SequenceArray<MethodSymbolArchetype>? _methods;

   /// <summary>
   /// Gets or sets the sequence of methods defined in the named type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the methods are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<MethodSymbolArchetype> Methods
   {
      get => LoadedFlags.Methods 
         ? _methods ?? throw new InvalidOperationException("Methods should be loaded but is null.")
         : throw new InvalidOperationException("Methods are not loaded.");
      set
      {
         _methods = value;
         LoadedFlags.Methods = true;
      }
   }
   
   private SequenceArray<PropertySymbolArchetype>? _properties;

   /// <summary>
   /// Gets or sets the sequence of properties defined in the named type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the properties are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<PropertySymbolArchetype> Properties
   {
      get => LoadedFlags.Properties 
         ? _properties ?? throw new InvalidOperationException("Properties should be loaded but is null.")
         : throw new InvalidOperationException("Properties are not loaded.");
      set
      {
         _properties = value;
         LoadedFlags.Properties = true;
      }
   }
   
   private SequenceArray<FieldSymbolArchetype>? _fields;

   /// <summary>
   /// Gets or sets the sequence of fields defined in the named type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the fields are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<FieldSymbolArchetype> Fields
   {
      get => LoadedFlags.Fields 
         ? _fields ?? throw new InvalidOperationException("Fields should be loaded but is null.")
         : throw new InvalidOperationException("Fields are not loaded.");
      set
      {
         _fields = value;
         LoadedFlags.Fields = true;
      }
   }
   
   private SequenceArray<TypeParameterArchetype>? _typeParameters;

   /// <summary>
   /// Gets or sets the sequence of type parameters for a generic named type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the type parameters are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<TypeParameterArchetype> TypeParameters
   {
      get => LoadedFlags.TypeParameters 
         ? _typeParameters ?? throw new InvalidOperationException("Type parameters should be loaded but is null.")
         : throw new InvalidOperationException("Type parameters are not loaded.");
      set
      {
         _typeParameters = value;
         LoadedFlags.TypeParameters = true;
      }
   }
   
   private SequenceArray<TypeSymbolArchetype>? _typeArguments;

   /// <summary>
   /// Gets or sets the sequence of type arguments for a generic named type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the type arguments are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<TypeSymbolArchetype> TypeArguments
   {
      get => LoadedFlags.TypeArguments 
         ? _typeArguments ?? throw new InvalidOperationException("Type arguments should be loaded but is null.")
         : throw new InvalidOperationException("Type arguments are not loaded.");
      set
      {
         _typeArguments = value;
         LoadedFlags.TypeArguments = true;
      }
   }
   
   private SequenceArray<NullableAnnotation>? _typeArgumentNullableAnnotations;

   /// <summary>
   /// Gets or sets the sequence of nullable annotations for type arguments of the named type.
   /// </summary>
   /// <exception cref="InvalidOperationException">Thrown if the annotations are accessed before they are loaded or are unexpectedly null.</exception>
   public SequenceArray<NullableAnnotation> TypeArgumentNullableAnnotations
   {
      get => LoadedFlags.TypeArgumentNullableAnnotations 
         ? _typeArgumentNullableAnnotations ?? throw new InvalidOperationException("Type argument nullable annotations should be loaded but is null.")
         : throw new InvalidOperationException("Type argument nullable annotations are not loaded.");
      set
      {
         _typeArgumentNullableAnnotations = value;
         LoadedFlags.TypeArgumentNullableAnnotations = true;
      }
   }
   
   private SequenceArray<IAttributeSpec>? _attributes;

   /// <summary>
   /// Gets or sets the sequence of attributes applied to the named type.
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
/// Represents the lazy-loading state flags for named type symbol properties.
/// </summary>
public record struct NamedTypeSymbolLoadFlags
{
   private PackedBools8 Flags;

   /// <summary>
   /// Gets or sets a value indicating whether the methods have been loaded.
   /// </summary>
   public bool Methods
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the type parameters have been loaded.
   /// </summary>
   public bool TypeParameters
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the type arguments have been loaded.
   /// </summary>
   public bool TypeArguments
   {
      get => Flags.Get(2);
      set => Flags.Set(2, value);
   }
 
   /// <summary>
   /// Gets or sets a value indicating whether the type argument nullable annotations have been loaded.
   /// </summary>
   public bool TypeArgumentNullableAnnotations
   {
      get => Flags.Get(3);
      set => Flags.Set(3, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the attributes have been loaded.
   /// </summary>
   public bool Attributes
   {
      get => Flags.Get(4);
      set => Flags.Set(4, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the properties have been loaded.
   /// </summary>
   public bool Properties
   {
      get => Flags.Get(5);
      set => Flags.Set(5, value);
   }
   
   /// <summary>
   /// Gets or sets a value indicating whether the fields have been loaded.
   /// </summary>
   public bool Fields
   {
      get => Flags.Get(6);
      set => Flags.Set(6, value);
   }
}
