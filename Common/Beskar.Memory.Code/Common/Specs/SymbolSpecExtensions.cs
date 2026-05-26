using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Common.Specs;

/// <summary>
/// Provides extension members for <see cref="SymbolSpec"/> to aid in resolving standard namespaces and generating filenames.
/// </summary>
public static class SymbolSpecExtensions
{
   extension(SymbolSpec spec)
   {
      /// <summary>
      /// Gets the standard generated file path name for this specification.
      /// </summary>
      public string GeneratedFilePath => $"{(spec.NameSpace is not { Length: > 0 } ? "Global" : spec.NameSpace)}.{spec.Name}.g.cs";
      
      /// <summary>
      /// Gets a value indicating whether this symbol spec represents a <see cref="Guid"/>.
      /// </summary>
      public bool IsGuid => spec is { Name: "Guid", IsInSystemNamespace: true };
      
      /// <summary>
      /// Gets a value indicating whether the symbol resides in the <c>System</c> namespace.
      /// </summary>
      public bool IsInSystemNamespace => spec.NameSpace is "System";

      /// <summary>
      /// Gets a value indicating whether the symbol resides in the <c>System.Threading.Tasks</c> namespace.
      /// </summary>
      public bool IsInSystemTasksNamespace => spec.NameSpace is "System.Threading.Tasks";
   }
}
