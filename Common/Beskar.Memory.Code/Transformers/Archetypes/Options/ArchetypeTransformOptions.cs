using System.Diagnostics.CodeAnalysis;
using Beskar.Memory.Code.Interfaces.Specs;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Collections;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Transformers.Archetypes.Options;

/// <summary>
/// Provides transformation options and caching mechanisms for converting compiler symbols into archetypes.
/// </summary>
public sealed class ArchetypeTransformOptions
{
   /// <summary>
   /// Gets or sets the field transformation options.
   /// </summary>
   public FieldTransformOptions Fields { get; set; } = FieldTransformOptions.Minimal;
   
   /// <summary>
   /// Gets or sets the method transformation options.
   /// </summary>
   public MethodTransformOptions Methods { get; set; } = MethodTransformOptions.Minimal;
   
   /// <summary>
   /// Gets or sets the property transformation options.
   /// </summary>
   public PropertyTransformOptions Properties { get; set; } = PropertyTransformOptions.Minimal;
   
   /// <summary>
   /// Gets or sets the named type transformation options.
   /// </summary>
   public NamedTypeTransformOptions NamedTypes { get; set; } = NamedTypeTransformOptions.Minimal;
   
   /// <summary>
   /// Gets or sets the type parameter transformation options.
   /// </summary>
   public TypeParameterTransformOptions TypeParameters { get; set; } = TypeParameterTransformOptions.Minimal;
   
   /// <summary>
   /// Gets or sets the type transformation options.
   /// </summary>
   public TypeTransformOptions Types { get; set; } = TypeTransformOptions.Minimal;
   
   /// <summary>
   /// Gets or sets the symbol base transformation options.
   /// </summary>
   public SymbolTransformOptions Symbols { get; set; } = SymbolTransformOptions.Minimal;
   
   /// <summary>
   /// Gets or sets the parameter transformation options.
   /// </summary>
   public ParameterTransformOptions Parameters { get; set; } = ParameterTransformOptions.Minimal;

   private readonly Dictionary<string, Func<ISymbol, AttributeData, IAttributeSpec>> _attributeFactories = [];
   private readonly Dictionary<Type, object> _symbolCaches = [];

   /// <summary>
   /// Determines if an attribute is relevant based on its full name.
   /// </summary>
   /// <param name="fullName">The full name of the attribute type.</param>
   /// <returns><c>true</c> if the attribute is relevant and registered; otherwise, <c>false</c>.</returns>
   public bool IsAttributeRelevant(string? fullName)
   {
      return fullName is not null && _attributeFactories.ContainsKey(fullName);
   }
   
   /// <summary>
   /// Retrieves the registered attribute factory for the specified attribute name.
   /// </summary>
   /// <param name="fullName">The full name of the attribute type.</param>
   /// <returns>A factory function to create an <see cref="IAttributeSpec"/>.</returns>
   /// <exception cref="InvalidOperationException">Thrown if no factory is registered for the specified attribute.</exception>
   public Func<ISymbol, AttributeData, IAttributeSpec> GetAttributeFactory(string fullName)
   {
      return _attributeFactories.GetValueOrDefault(fullName)
         ?? throw new InvalidOperationException("Attribute factory not found");
   }

   /// <summary>
   /// Resolves and creates a sequence of attribute specifications for a given symbol from compiler attributes.
   /// </summary>
   /// <param name="symbol">The compiler symbol the attributes are applied to.</param>
   /// <param name="attributes">The collection of attribute data to process.</param>
   /// <returns>A sequence of resolved <see cref="IAttributeSpec"/> instances.</returns>
   public SequenceArray<IAttributeSpec> GetAttributes(ISymbol symbol, IEnumerable<AttributeData> attributes)
   {
      return [.. 
         attributes
            .Select(x => new { FullName = x.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), Data = x })
            .Where(x => IsAttributeRelevant(x.FullName))
            .Select(x => GetAttributeFactory(x.FullName ?? string.Empty)(symbol, x.Data))
      ];
   }
   
   /// <summary>
   /// Registers an attribute factory for a specific attribute type.
   /// </summary>
   /// <typeparam name="T">The type of attribute specification.</typeparam>
   /// <param name="fullName">The full name of the attribute class.</param>
   /// <param name="factory">The factory function to instantiate the attribute specification.</param>
   /// <returns>The current <see cref="ArchetypeTransformOptions"/> instance for chaining.</returns>
   public ArchetypeTransformOptions RegisterAttribute<T>(string fullName, Func<ISymbol, AttributeData, T> factory)
      where T : IAttributeSpec
   {
      _attributeFactories.Add(fullName, (symbol, data) => factory(symbol, data));
      return this;
   }
   
   /// <summary>
   /// Attempts to get a cached archetype transformation for the specified symbol.
   /// </summary>
   /// <typeparam name="TSymbol">The type of the compiler symbol.</typeparam>
   /// <typeparam name="T">The type of the archetype.</typeparam>
   /// <param name="symbol">The compiler symbol to look up.</param>
   /// <param name="archetype">When this method returns, contains the cached archetype, if found; otherwise, the default value.</param>
   /// <returns><c>true</c> if the archetype was found in the cache; otherwise, <c>false</c>.</returns>
   public bool TryGetCached<TSymbol, T>(TSymbol symbol, [MaybeNullWhen(false)] out T archetype)
      where TSymbol : ISymbol
   {
      return GetSymbolCache<T>().TryGetValue(symbol, out archetype);
   }

   /// <summary>
   /// Adds a transformed archetype to the cache for the specified symbol.
   /// </summary>
   /// <typeparam name="TSymbol">The type of the compiler symbol.</typeparam>
   /// <typeparam name="T">The type of the archetype.</typeparam>
   /// <param name="symbol">The compiler symbol.</param>
   /// <param name="archetype">The transformed archetype to cache.</param>
   public void AddToCache<TSymbol, T>(TSymbol symbol, T archetype)
      where TSymbol : ISymbol
   {
      GetSymbolCache<T>()[symbol] = archetype;
   }

   /// <summary>
   /// Clears all cached symbol transformations.
   /// </summary>
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
