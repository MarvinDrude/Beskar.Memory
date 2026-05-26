using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Beskar.Memory.Results;

/// <summary>
/// Represents the result of an operation that can either succeed with a value of type <typeparamref name="TSuccess"/>
/// or fail with an error of type <typeparamref name="TError"/>.
/// </summary>
public readonly struct Result<TSuccess, TError> : IEquatable<Result<TSuccess, TError>>
{
   /// <summary>
   /// Gets a value indicating whether the operation succeeded.
   /// </summary>
   public readonly bool IsSuccess;
   
   /// <summary>
   /// Gets a value indicating whether the result has a success value.
   /// </summary>
   [MemberNotNullWhen(true, nameof(Success))]
   [MemberNotNullWhen(false, nameof(Error))]
   public bool HasValue => IsSuccess;
   
   /// <summary>
   /// Gets a value indicating whether the operation failed.
   /// </summary>
   [MemberNotNullWhen(true, nameof(Error))]
   [MemberNotNullWhen(false, nameof(Success))]
   public bool Failed => !IsSuccess;

   /// <summary>
   /// The success value, present only when <see cref="IsSuccess"/> is true.
   /// </summary>
   public readonly TSuccess? Success;

   /// <summary>
   /// The error value, present only when <see cref="IsSuccess"/> is false.
   /// </summary>
   public readonly TError? Error;

   /// <summary>
   /// Initializes a new successful result with the specified value.
   /// </summary>
   public Result(TSuccess success)
   {
      IsSuccess = true;
      Success = success;
      Error = default;
   }

   /// <summary>
   /// Initializes a new failed result with the specified error.
   /// </summary>
   public Result(TError error)
   {
      IsSuccess = false;
      Error = error;
      Success = default;
   }

   /// <summary>
   /// Initializes a default failed result.
   /// </summary>
   public Result()
   {
      IsSuccess = false;
      Success = default;
      Error = default;
   }

   /// <summary>
   /// Deconstructs the result into success state, value, and error.
   /// </summary>
   public void Deconstruct(out bool isSuccess, out TSuccess? success, out TError? error)
   {
      isSuccess = IsSuccess;
      success = Success;
      error = Error;
   }

   /// <inheritdoc />
   public override string ToString()
   {
      return HasValue 
         ? $"SUCCESS: {Success?.ToString() ?? "null"}" 
         : $"ERROR: {Error?.ToString() ?? "null"}";
   }

   /// <inheritdoc />
   public bool Equals(Result<TSuccess, TError> other)
   {
      if (IsSuccess != other.IsSuccess)
      {
         return false;
      }

      return IsSuccess 
         ? EqualityComparer<TSuccess>.Default.Equals(Success, other.Success)
         : EqualityComparer<TError>.Default.Equals(Error, other.Error);
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is Result<TSuccess, TError> other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return IsSuccess 
         ? HashCode.Combine(true, Success) 
         : HashCode.Combine(false, Error);
   }

   /// <summary>
   /// Compares two instances for equality.
   /// </summary>
   public static bool operator ==(Result<TSuccess, TError> left, Result<TSuccess, TError> right)
   {
      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances for inequality.
   /// </summary>
   public static bool operator !=(Result<TSuccess, TError> left, Result<TSuccess, TError> right)
   {
      return !left.Equals(right);
   }

   /// <summary>
   /// Implicitly converts a success value into a successful result.
   /// </summary>
   public static implicit operator Result<TSuccess, TError>(TSuccess success) => new(success);

   /// <summary>
   /// Implicitly converts an error value into a failed result.
   /// </summary>
   public static implicit operator Result<TSuccess, TError>(TError error) => new(error);
}
