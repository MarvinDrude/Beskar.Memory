using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Beskar.Memory.Code.TypeIdGenerator.Interfaces;

namespace Beskar.Memory.Code.TypeIdGenerator.Internal;

[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{DebuggerView,nq}")]
internal readonly partial record struct ExampleNumberId(Guid Value)
   : IComparable<ExampleNumberId>, ISpanParsable<ExampleNumberId>, ITypeSafeIdentifier<Guid>, ISpanFormattable
{
   // Static constant like fields
   public static ExampleNumberId Empty { get; } = new (Guid.Empty);

   // Check properties
   public bool IsEmpty => Value.Equals(Guid.Empty);
   public bool HasValue => !IsEmpty;
   
   // Stringify and debugger view
   public override string ToString() => $"{nameof(ExampleNumberId)}: {Value}";
   internal string DebuggerView => ToString();
   
   public bool Equals(ExampleNumberId other)
   {
      return Value == other.Value;
   }
   
   public bool Equals(ITypeSafeIdentifier<Guid>? other)
   {
      return other is ExampleNumberId id && Value == id.Value;
   }
   
   public override int GetHashCode()
   {
      return Value.GetHashCode();
   }
   
   // Comparable interface
   public int CompareTo(ExampleNumberId other) => Value.CompareTo(other.Value);
   
   public int CompareTo(ITypeSafeIdentifier<Guid>? other)
   {
      return other switch 
      { 
         null => 1, 
         ExampleNumberId id => Value.CompareTo(id.Value), 
         _ => throw new ArgumentException($"Object must be of type ExampleNumberId.") 
      }; 
   }
   
   // Compare operators
   public static bool operator <(ExampleNumberId left, ExampleNumberId right) => left.Value < right.Value; 
   public static bool operator <=(ExampleNumberId left, ExampleNumberId right) => left.Value <= right.Value; 
   public static bool operator >(ExampleNumberId left, ExampleNumberId right) => left.Value > right.Value; 
   public static bool operator >=(ExampleNumberId left, ExampleNumberId right) => left.Value >= right.Value; 
   
   // Implicit non-nullable conversions
   public static implicit operator Guid(ExampleNumberId id) => id.Value;
   public static implicit operator ExampleNumberId(Guid value) => new(value);
   
   // Implicit nullable conversions
   public static implicit operator Guid?(ExampleNumberId? id) => id?.Value;
   public static implicit operator ExampleNumberId?(Guid? value) => value.HasValue ? new ExampleNumberId(value.Value) : null;
   
   // Implicit non-nullable to nullable conversions
   public static implicit operator Guid?(ExampleNumberId id) => id.Value;
   public static implicit operator ExampleNumberId?(Guid value) => new ExampleNumberId(value);
   
   // Explicit nullable to non-nullable conversions
   public static explicit operator Guid(ExampleNumberId? id) => id ?? Empty;
   public static explicit operator ExampleNumberId(Guid? value) => value.HasValue ? new ExampleNumberId(value.Value) : Empty;
   
   // Span parsable interface
   public static ExampleNumberId Parse(string s, IFormatProvider? provider)
      => Parse(s.AsSpan(), provider);

   public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ExampleNumberId result)
      => TryParse(s.AsSpan(), provider, out result);

   public static ExampleNumberId Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
   {
      return new ExampleNumberId(Guid.Parse(s, provider));
   }

   public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ExampleNumberId result)
   {
      if (Guid.TryParse(s, provider, out var value))
      {
         result = new ExampleNumberId(value);
         return true;
      }
      
      result = default;
      return false;
   }
   
   // Span formattable
   public string ToString(string? format, IFormatProvider? formatProvider)
      => Value.ToString(format, formatProvider);

   public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
      => Value.TryFormat(destination, out charsWritten, format);
}