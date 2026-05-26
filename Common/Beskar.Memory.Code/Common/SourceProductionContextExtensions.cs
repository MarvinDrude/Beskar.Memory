using Beskar.Memory.Code.Models.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common;

public static class SourceProductionContextExtensions
{
   extension(SourceProductionContext context)
   {
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

