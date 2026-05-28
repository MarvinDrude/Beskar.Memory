using System;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Serialization.Serializers.Collections;

/// <summary>
/// Provides utility methods to validate collection structures and lengths during deserialization.
/// </summary>
internal static class CollectionValidation
{
   /// <summary>
   /// Validates a collection length against the active deserialization context's configured limit.
   /// </summary>
   /// <param name="length">The deserialized length of the collection.</param>
   /// <exception cref="InvalidOperationException">Thrown if length exceeds maximum configured length.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void ValidateLength(long length)
   {
      var max = DeserializationContext.Current.Options.MaxCollectionLength;
      if (length > max)
      {
         throw new InvalidOperationException($"Collection length {length} exceeds maximum allowed length of {max}.");
      }
   }
}
