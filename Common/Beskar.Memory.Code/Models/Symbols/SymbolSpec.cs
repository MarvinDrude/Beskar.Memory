using System.Diagnostics;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Flags;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Models.Symbols;

[DebuggerDisplay("{FullName, nq}")]
public sealed record SymbolSpec
{
   public required string Name { get; init; }
   public required string MetadataName { get; init; }
   public required string FullName { get; init; }
   
   public string? NameSpace { get; init; }
   
   public required SymbolKind Kind { get; init; }
   public required Accessibility Accessibility { get; init; }
   
   private PackedBools8 _flags;
   
   private SymbolLoadFlags _loadedFlags;
   private ref SymbolLoadFlags LoadedFlags => ref _loadedFlags;

   public bool IsStatic
   {
      get => _flags.Get(0);
      set => _flags.Set(0, value);
   }

   public bool IsAbstract
   {
      get => _flags.Get(1);
      set => _flags.Set(1, value);
   }

   public bool IsVirtual
   {
      get => _flags.Get(2);
      set => _flags.Set(2, value);
   }

   public bool IsSealed
   {
      get => _flags.Get(3);
      set => _flags.Set(3, value);
   }

   public bool IsOverride
   {
      get => _flags.Get(4);
      set => _flags.Set(4, value);
   }
   
   public bool IsImplicitlyDeclared
   {
      get => _flags.Get(5);
      set => _flags.Set(5, value);
   }
   
   public bool IsExtern
   {
      get => _flags.Get(6);
      set => _flags.Set(6, value);
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

public record struct SymbolLoadFlags
{
   private PackedBools8 Flags;
   
   public bool Attributes
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }
}

