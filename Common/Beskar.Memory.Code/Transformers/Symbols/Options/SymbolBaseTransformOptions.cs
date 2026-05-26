namespace Beskar.Memory.Code.Transformers.Symbols.Options;

/// <summary>
/// Provides a base class for symbol transformation options, managing recursive depth and loaded feature flags.
/// </summary>
/// <typeparam name="TFlags">The type of the state/feature flags struct.</typeparam>
public abstract class SymbolBaseTransformOptions<TFlags>
   where TFlags : struct
{
   /// <summary>
   /// Gets or sets the maximum recursive depth to transform nested symbols.
   /// </summary>
   public int Depth { get; set; } = 1;
   
   private TFlags _loadedFlags;

   /// <summary>
   /// Gets a reference to the loaded feature flags.
   /// </summary>
   public ref TFlags Load => ref _loadedFlags;
}
