using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Formats;

/// <summary>
/// Provides predefined formatting configurations for rendering symbols in code generation.
/// </summary>
public static class SymbolDisplayFormats
{
   /// <summary>
   /// A format configuration that renders the fully qualified name without any generic parameters.
   /// </summary>
   public static readonly SymbolDisplayFormat FullyQualifiedNoGenerics =
      new (
         globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
         typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
         genericsOptions: SymbolDisplayGenericsOptions.None,
         miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable
      );
}
