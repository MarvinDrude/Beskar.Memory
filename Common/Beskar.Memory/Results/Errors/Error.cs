using System;
using System.Collections.Generic;

namespace Beskar.Memory.Results.Errors;

/// <summary>
/// Represents a strongly-typed generic error wrapper containing a specific error detail of type <typeparamref name="T1"/>.
/// </summary>
public readonly struct Error<T1> : IEquatable<Error<T1>>
{
   /// <summary>
   /// Gets the strong-typed detail of this error.
   /// </summary>
   public readonly T1 Detail;

   /// <summary>
   /// Initializes a new instance of the <see cref="Error{T1}"/> struct with the given detail.
   /// </summary>
   public Error(T1 detail)
   {
      Detail = detail;
   }

   /// <inheritdoc />
   public override string ToString()
   {
      return Detail?.ToString() ?? "null";
   }

   /// <inheritdoc />
   public bool Equals(Error<T1> other)
   {
      return EqualityComparer<T1>.Default.Equals(Detail, other.Detail);
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is Error<T1> other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return Detail?.GetHashCode() ?? 0;
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(Error<T1> left, Error<T1> right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(Error<T1> left, Error<T1> right)
   {
      return !left.Equals(right);
   }

   /// <summary>
   /// Implicitly converts an error detail into an Error wrapper.
   /// </summary>
   public static implicit operator Error<T1>(T1 detail) => new(detail);
}
