namespace Beskar.Memory.Code.Transformers.Symbols.Options;

public abstract class SymbolBaseTransformOptions<TFlags>
   where TFlags : struct
{
   public int Depth { get; set; } = 1;
   
   private TFlags _loadedFlags;
   public ref TFlags Load => ref _loadedFlags;
}

