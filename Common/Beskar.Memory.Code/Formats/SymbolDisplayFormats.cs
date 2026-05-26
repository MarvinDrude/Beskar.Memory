using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Formats;

public static class SymbolDisplayFormats
{
   public static readonly SymbolDisplayFormat FullyQualifiedNoGenerics =
      new (
         globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
         typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
         genericsOptions: SymbolDisplayGenericsOptions.None,
         miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable
      );
}

