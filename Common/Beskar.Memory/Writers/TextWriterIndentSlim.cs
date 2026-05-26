using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Beskar.Memory.Writers;

/// <summary>
/// A high-performance, stack-only <see langword="ref struct"/> writer for formatted text with automatic indentation.
/// Supports writing to an initial stack-allocated span and growing onto rented memory from an array pool if needed.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public ref partial struct TextWriterIndentSlim : IDisposable
{
   /// <summary>
   /// Gets a read-only span representing the already written data.
   /// </summary>
   public readonly ReadOnlySpan<char> WrittenSpan
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _buffer.WrittenSpan;
   }

   /// <summary>
   /// Gets the character used for indentation.
   /// </summary>
   public char IndentCharacter { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

   /// <summary>
   /// Gets the character used for a new line.
   /// </summary>
   public char NewLineCharacter { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

   /// <summary>
   /// Gets the size (number of characters) for a single indentation level.
   /// </summary>
   public int IndentSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

   /// <summary>
   /// Gets the current indentation level.
   /// </summary>
   public int CurrentIndentLevel
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _currentLevel;
   }
   
   private BufferWriter<char> _indentCache;
   private ReadOnlySpan<char> _currentLevelBuffer;
   private int _currentLevel;

   private BufferWriter<char> _buffer;

   /// <summary>
   /// Initializes a new instance of the <see cref="TextWriterIndentSlim"/> struct.
   /// </summary>
   /// <param name="buffer">The initial buffer to write text into.</param>
   /// <param name="indentBuffer">A buffer to cache indentation levels.</param>
   /// <param name="indentSize">The size of each indentation level.</param>
   /// <param name="indentChar">The character to use for indentation.</param>
   /// <param name="newLineChar">The character to use for new lines.</param>
   /// <param name="initialMinGrowCapacity">The initial minimum growth capacity of the internal buffer.</param>
   public TextWriterIndentSlim(
      Span<char> buffer,
      Span<char> indentBuffer,
      int indentSize = DefaultIndentSize,
      char indentChar = DefaultIndent,
      char newLineChar = DefaultNewLine,
      int initialMinGrowCapacity = 1024)
   {
      NewLineCharacter = newLineChar;
      IndentCharacter = indentChar;
      IndentSize = indentSize;
      
      _buffer = new BufferWriter<char>(buffer, initialMinGrowCapacity);
      _currentLevel = 0;
      
      _indentCache = new BufferWriter<char>(indentBuffer);
      _indentCache.Fill(IndentCharacter);
      _currentLevelBuffer = [];
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="TextWriterIndentSlim"/> struct with an automatically allocated indentation cache buffer.
   /// </summary>
   /// <param name="buffer">The initial buffer to write text into.</param>
   /// <param name="indentSize">The size of each indentation level.</param>
   /// <param name="indentChar">The character to use for indentation.</param>
   /// <param name="newLineChar">The character to use for new lines.</param>
   /// <param name="initialMinGrowCapacity">The initial minimum growth capacity of the internal buffer.</param>
   public TextWriterIndentSlim(
      Span<char> buffer,
      int indentSize = DefaultIndentSize,
      char indentChar = DefaultIndent,
      char newLineChar = DefaultNewLine,
      int initialMinGrowCapacity = 1024)
      : this(buffer, new char[128], indentSize, indentChar, newLineChar, initialMinGrowCapacity)
   {
   }

   /// <summary>
   /// Writes a new line character.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine()
   {
      _buffer.Add(NewLineCharacter);
   }

   /// <summary>
   /// Writes a new line character if the condition is met.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLineIf(bool condition)
   {
      if (condition)
      {
         WriteLine();
      }
   }

   /// <summary>
   /// Writes the specified character span followed by a new line character if the condition is met.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLineIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (condition)
      {
         WriteLine(content, multiLine);
      }
   }

   /// <summary>
   /// Writes the specified character span followed by a new line character.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(scoped Span<char> content, bool multiLine = false)
   {
      Write(content, multiLine);
      WriteLine();
   }
   
   /// <summary>
   /// Writes the specified read-only character span followed by a new line character.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(scoped ReadOnlySpan<char> content, bool multiLine = false)
   {
      Write(content, multiLine);
      WriteLine();
   }

   /// <summary>
   /// Writes the specified string followed by a new line character.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(string text, bool multiLine = false)
   {
      Write(text.AsSpan(), multiLine);
      WriteLine();
   }
   
   /// <summary>
   /// Writes a string text.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(string text)
   {
      WriteText(text.AsSpan());
   }
   
   /// <summary>
   /// Writes a read-only character span text.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(scoped ReadOnlySpan<char> text)
   {
      AddIndentOnDemand();
      _buffer.Write(text);
   }

   /// <summary>
   /// Writes a read-only span of characters, optionally handling multiple lines.
   /// </summary>
   public void Write(scoped ReadOnlySpan<char> text, bool multiLine = false)
   {
      if (!multiLine)
      {
         WriteText(text);
      }
      else
      {
         while (text.Length > 0)
         {
            var newLinePos = text.IndexOf(NewLineCharacter);

            if (newLinePos >= 0)
            {
               var line = text[..newLinePos];
               
               WriteIf(!line.IsEmpty, line);
               WriteLine();

               text = text[(newLinePos + 1)..];
            }
            else
            {
               WriteText(text);
               break;
            }
         }
      }
   }

   /// <summary>
   /// Writes a string of characters, optionally handling multiple lines.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Write(string text, bool multiLine = false)
   {
      Write(text.AsSpan(), multiLine);
   }

   /// <summary>
   /// Writes a read-only character span if the condition is met.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (condition)
      {
         Write(content, multiLine);
      }  
   }
   
   /// <summary>
   /// Increases the current indentation level.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void UpIndent()
   {
      _currentLevel++;
      _currentLevelBuffer = GetCurrentIndentBuffer();
   }

   /// <summary>
   /// Decreases the current indentation level.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void DownIndent()
   {
      _currentLevel--;
      ArgumentOutOfRangeException.ThrowIfLessThan(_currentLevel, 0, nameof(_currentLevel));
      
      _currentLevelBuffer = GetCurrentIndentBuffer();
   }
   
   /// <summary>
   /// Acquires a span of the specified length.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<char> AcquireSpan(int length)
   {
      return _buffer.AcquireSpan(length, true);
   }
   
   /// <summary>
   /// Acquires a span of the specified length, automatically applying the current indentation level on demand first.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<char> AcquireSpanIndented(int length)
   {
      AddIndentOnDemand();
      return _buffer.AcquireSpan(length, true);
   }
   
   private void AddIndentOnDemand()
   {
      if (_currentLevelBuffer.IsEmpty)
      {
         return;
      }
      
      if (_buffer.Position == 0 || _buffer.WrittenSpan[^1] == NewLineCharacter)
      {
         _buffer.Write(_currentLevelBuffer);
      }
   }
   
   /// <summary>
   /// Gets a read-only character span representing the current indentation prefix.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<char> GetCurrentIndentBuffer()
   {
      if (_currentLevel == 0)
      {
         return [];
      }

      var levelCount = IndentSize * _currentLevel;
      
      while (_indentCache.Position < levelCount)
      {
         _indentCache.Add(IndentCharacter);
      }

      return _indentCache.WrittenSpan[..levelCount];
   }
   
   /// <summary>
   /// Returns a string representation of the written text content.
   /// </summary>
   public override string ToString()
   {
      return _buffer.WrittenSpan.ToString();
   }
   
   /// <summary>
   /// Disposes the writer, returning rented memory blocks.
   /// </summary>
   public void Dispose()
   {
      _buffer.Dispose();
      _indentCache.Dispose();
   }
}
