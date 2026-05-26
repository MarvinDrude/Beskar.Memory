using Beskar.Memory.Code.Models.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common;

/// <summary>
/// Provides extension members for <see cref="SourceProductionContext"/> to dispatch diagnostic information during source generation.
/// </summary>
public static class SourceProductionContextExtensions
{
   extension(SourceProductionContext context)
   {
      /// <summary>
      /// Dispatches diagnostic specifications to the source production context by matching them against defined descriptors.
      /// </summary>
      /// <typeparam name="T">The type of the target validation value.</typeparam>
      /// <param name="descriptors">A dictionary of defined <see cref="DiagnosticDescriptor"/> objects mapped by their diagnostic IDs.</param>
      /// <param name="maybeSpec">The diagnostic container containing diagnostic results.</param>
      public void DispatchDiagnostics<T>(
         Dictionary<string, DiagnosticDescriptor> descriptors,
         MaybeSpec<T> maybeSpec)
      {
         if (maybeSpec.Diagnostics.Array.Length == 0)
         {
            return;
         }

         foreach (var diagnostic in maybeSpec.Diagnostics)
         {
            if (!descriptors.TryGetValue(diagnostic.DiagnosticId, out var descriptor))
            {
               continue;
            }

            var args = diagnostic.Arguments.Array.Cast<object>().ToArray();
            context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, args));
         }
      }
   }
}
