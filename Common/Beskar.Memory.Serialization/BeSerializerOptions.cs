using System;

namespace Beskar.Memory.Serialization;

/// <summary>
/// Represents configuration options for serialization and deserialization in Beskar.Memory.
/// </summary>
public sealed class BeSerializerOptions
{
   /// <summary>
   /// Gets the default singleton instance of serialization options.
   /// </summary>
   public static BeSerializerOptions Default { get; } = new();

   /// <summary>
   /// Gets or sets the maximum depth allowed during serialization and deserialization to prevent stack overflows.
   /// Default is 64.
   /// </summary>
   public int MaxDepth { get; set; } = 64;

   /// <summary>
   /// Gets or sets the maximum collection length allowed during deserialization to prevent memory exhaustion.
   /// Default is 100,000.
   /// </summary>
   public int MaxCollectionLength { get; set; } = 100_000;
}
