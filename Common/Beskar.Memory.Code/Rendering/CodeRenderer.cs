using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Rendering;

/// <summary>
/// Provides a base class for rendering generated source code within a source generator context.
/// </summary>
/// <param name="ctx">The <see cref="SourceProductionContext"/> used for source generation and cancellation monitoring.</param>
public abstract class CodeRenderer(SourceProductionContext ctx)
{
   private readonly SourceProductionContext _ctx = ctx;

   /// <summary>
   /// Renders the generated source code as a string.
   /// </summary>
   /// <returns>A string containing the rendered source code.</returns>
   protected abstract string Render();

   /// <summary>
   /// Renders the source code and adds it to the generator context with the specified file name.
   /// </summary>
   /// <param name="fileName">The name of the source file to add to the compilation.</param>
   /// <exception cref="OperationCanceledException">Thrown if the source generation operation is canceled.</exception>
   public void Render(string fileName)
   {
      _ctx.CancellationToken.ThrowIfCancellationRequested();
      _ctx.AddSource(fileName, Render());
   }
}
