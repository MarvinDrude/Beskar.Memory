using System;

namespace Beskar.Memory.Results.Errors;

/// <summary>
/// Represents a simple error with a string detail message.
/// </summary>
public readonly struct StringError(string detail) : IEquatable<StringError>
{
   /// <summary>
   /// Gets the error detail message.
   /// </summary>
   public readonly string Detail = detail ?? string.Empty;

   /// <inheritdoc />
   public override string ToString() => Detail;

   /// <inheritdoc />
   public bool Equals(StringError other)
   {
      return string.Equals(Detail, other.Detail, StringComparison.Ordinal);
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is StringError other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return Detail.GetHashCode();
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(StringError left, StringError right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(StringError left, StringError right)
   {
      return !left.Equals(right);
   }

   /// <summary>
   /// Implicitly converts a string into a StringError.
   /// </summary>
   public static implicit operator StringError(string detail) => new(detail);
}
