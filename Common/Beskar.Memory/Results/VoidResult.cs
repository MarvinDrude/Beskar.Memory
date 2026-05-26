using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Beskar.Memory.Results;

/// <summary>
/// Represents the result of an operation that has no success value, but can fail with an error of type <typeparamref name="TError"/>.
/// </summary>
public readonly struct VoidResult<TError> : IEquatable<VoidResult<TError>>
{
   /// <summary>
   /// Gets a value indicating whether the operation succeeded.
   /// </summary>
   public readonly bool IsSuccess;
   
   /// <summary>
   /// Gets a value indicating whether the operation succeeded.
   /// </summary>
   [MemberNotNullWhen(false, nameof(Error))]
   public bool HasValue => IsSuccess;
   
   /// <summary>
   /// Gets a value indicating whether the operation failed.
   /// </summary>
   [MemberNotNullWhen(true, nameof(Error))]
   public bool Failed => !IsSuccess;
   
   /// <summary>
   /// The error value, present only when <see cref="IsSuccess"/> is false.
   /// </summary>
   public readonly TError? Error;

   /// <summary>
   /// Initializes a successful void result.
   /// </summary>
   public VoidResult()
   {
      IsSuccess = true;
      Error = default;
   }

   /// <summary>
   /// Initializes a failed void result with the specified error.
   /// </summary>
   public VoidResult(TError error)
   {
      IsSuccess = false;
      Error = error;
   }

   /// <summary>
   /// Deconstructs the result into success state and error.
   /// </summary>
   public void Deconstruct(out bool isSuccess, out TError? error)
   {
      isSuccess = IsSuccess;
      error = Error;
   }

   /// <inheritdoc />
   public override string ToString()
   {
      return HasValue 
         ? "SUCCESS" 
         : $"ERROR: {Error?.ToString() ?? "null"}";
   }

   /// <inheritdoc />
   public bool Equals(VoidResult<TError> other)
   {
      if (IsSuccess != other.IsSuccess)
      {
         return false;
      }

      return IsSuccess || EqualityComparer<TError>.Default.Equals(Error, other.Error);
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is VoidResult<TError> other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return IsSuccess 
         ? HashCode.Combine(true) 
         : HashCode.Combine(false, Error);
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(VoidResult<TError> left, VoidResult<TError> right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(VoidResult<TError> left, VoidResult<TError> right)
   {
      return !left.Equals(right);
   }

   /// <summary>
   /// Implicitly converts a boolean success state into a void result.
   /// </summary>
   public static implicit operator VoidResult<TError>(bool success) => success ? new() : new(default!);

   /// <summary>
   /// Implicitly converts an error value into a failed void result.
   /// </summary>
   public static implicit operator VoidResult<TError>(TError error) => new(error);
}
