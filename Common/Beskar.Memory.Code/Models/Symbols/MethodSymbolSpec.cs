using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

[DebuggerDisplay("Method-Kind: {MethodKind, nq}")]
public sealed record MethodSymbolSpec
{
   public required MethodKind MethodKind { get; init; }
   private PackedBools8 _flags;
   
   private MethodSymbolLoadFlags _loadedFlags;
   private ref MethodSymbolLoadFlags LoadedFlags => ref _loadedFlags;
   
   public bool HasVoidReturn
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }
   
   public bool ReturnsByRef
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }
   
   public bool ReturnsByRefReadonly
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }
   
   public bool IsReadOnly
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }

   public bool IsIterator
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   public bool IsAsync
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }

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

public record struct MethodSymbolLoadFlags
{
   private PackedBools8 Flags;
   
   public bool ReturnType
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }

   public bool Parameters
   {
      get => Flags.Get(1);
      set => Flags.Set(1, value);
   }

   public bool Attributes
   {
      get => Flags.Get(2);
      set => Flags.Set(2, value);
   }
}

