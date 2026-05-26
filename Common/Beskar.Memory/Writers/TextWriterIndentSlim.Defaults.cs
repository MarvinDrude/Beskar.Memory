using System;

namespace Beskar.Memory.Writers;

public ref partial struct TextWriterIndentSlim
{
   private const char DefaultNewLine = '\n';
   private const char DefaultIndent = ' ';
   private const int DefaultIndentSize = 3;
}
