using System.Diagnostics.CodeAnalysis;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes.Options;

public sealed class ArchetypeTransformOptions
{
   public FieldTransformOptions Fields { get; set; } = FieldTransformOptions.Minimal;
   
   public MethodTransformOptions Methods { get; set; } = MethodTransformOptions.Minimal;
   
   public PropertyTransformOptions Properties { get; set; } = PropertyTransformOptions.Minimal;
   
   public NamedTypeTransformOptions NamedTypes { get; set; } = NamedTypeTransformOptions.Minimal;
   
   public TypeParameterTransformOptions TypeParameters { get; set; } = TypeParameterTransformOptions.Minimal;
   
   public TypeTransformOptions Types { get; set; } = TypeTransformOptions.Minimal;
   
   public SymbolTransformOptions Symbols { get; set; } = SymbolTransformOptions.Minimal;
   
   public ParameterTransformOptions Parameters { get; set; } = ParameterTransformOptions.Minimal;

   private readonly Dictionary<string, Func<ISymbol, AttributeData, IAttributeSpec>> _attributeFactories = [];
   private readonly Dictionary<Type, object> _symbolCaches = [];

   public bool IsAttributeRelevant(string? fullName)
   {
      return fullName is not null && _attributeFactories.ContainsKey(fullName);
   }
   
   public Func<ISymbol, AttributeData, IAttributeSpec> GetAttributeFactory(string fullName)
   {
      return _attributeFactories.GetValueOrDefault(fullName)
         ?? throw new InvalidOperationException("Attribute factory not found");
   }

   public SequenceArray<IAttributeSpec> GetAttributes(ISymbol symbol, IEnumerable<AttributeData> attributes)
   {
      return [.. 
         attributes
            .Select(x => new { FullName = x.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), Data = x })
            .Where(x => IsAttributeRelevant(x.FullName))
            .Select(x => GetAttributeFactory(x.FullName ?? string.Empty)(symbol, x.Data))
      ];
   }
   
   public ArchetypeTransformOptions RegisterAttribute<T>(string fullName, Func<ISymbol, AttributeData, T> factory)
      where T : IAttributeSpec
   {
      _attributeFactories.Add(fullName, (symbol, data) => factory(symbol, data));
      return this;
   }
   
   public bool TryGetCached<TSymbol, T>(TSymbol symbol, [MaybeNullWhen(false)] out T archetype)
      where TSymbol : ISymbol
   {
      return GetSymbolCache<T>().TryGetValue(symbol, out archetype);
   }

   public void AddToCache<TSymbol, T>(TSymbol symbol, T archetype)
      where TSymbol : ISymbol
   {
      GetSymbolCache<T>()[symbol] = archetype;
   }

   internal void ClearCache()
   {
      _symbolCaches.Clear();
   }

   private Dictionary<ISymbol, T> GetSymbolCache<T>()
   {
      var type = typeof(T);
      if (!_symbolCaches.TryGetValue(type, out var cache))
      {
         cache = new Dictionary<ISymbol, T>(SymbolEqualityComparer.Default);
         _symbolCaches[type] = cache;
      }
      
      return (Dictionary<ISymbol, T>)cache;
   }
}

