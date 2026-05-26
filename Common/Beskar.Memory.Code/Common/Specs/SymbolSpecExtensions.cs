using Beskar.Memory.Code.Models.Symbols;

namespace Beskar.Memory.Code.Common.Specs;

public static class SymbolSpecExtensions
{
   extension(SymbolSpec spec)
   {
      public string GeneratedFilePath => $"{(spec.NameSpace is not { Length: > 0 } ? "Global" : spec.NameSpace)}.{spec.Name}.g.cs";
      
      public bool IsGuid => spec is { Name: "Guid", IsInSystemNamespace: true };
      
      public bool IsInSystemNamespace => spec.NameSpace is "System";
      public bool IsInSystemTasksNamespace => spec.NameSpace is "System.Threading.Tasks";
   }
}

