using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Owners;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Serialization;

/// <summary>
/// Provides modern, ultra-high-performance, and highly convenient static utility methods for Beskar serialization.
/// </summary>
public static class BeSerializer
{
   #region Serialization

   /// <summary>
   /// Serializes the specified value into a fresh byte array.
   /// Uses a 256-byte stack-allocated buffer for single-pass serialization, automatically growing onto rented memory from the array pool if exceeded.
   /// </summary>
   /// <typeparam name="T">The type of the object to serialize.</typeparam>
   /// <param name="value">The value to serialize.</param>
   /// <returns>A byte array containing the serialized payload.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static byte[] Serialize<T>(scoped in T value)
   {
      Span<byte> initialBuffer = stackalloc byte[256];
      var writer = new BufferWriter<byte>(initialBuffer);

      var context = new SerializationContext();
      SerializationContext.Current = context;

      try
      {
         SerializerRegistry<T>.GetWrite()(ref writer, value);
         return writer.WrittenSpan.ToArray();
      }
      finally
      {
         SerializationContext.Current = default;
         context.Dispose();

         writer.Dispose();
      }
   }

   /// <summary>
   /// Serializes the specified value directly into the target span.
   /// </summary>
   /// <typeparam name="T">The type of the object to serialize.</typeparam>
   /// <param name="value">The value to serialize.</param>
   /// <param name="destination">The destination span to write the payload into.</param>
   /// <returns>The number of bytes written.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Serialize<T>(scoped in T value, Span<byte> destination)
   {
      var writer = new BufferWriter<byte>(destination);

      var context = new SerializationContext();

      SerializationContext.Current = context;
      try
      {
         SerializerRegistry<T>.GetWrite()(ref writer, value);
         return writer.Position;
      }
      finally
      {
         SerializationContext.Current = default;
         context.Dispose();

         writer.Dispose();
      }
   }

   /// <summary>
   /// Serializes the specified value directly into an existing high-performance <see cref="BufferWriter{byte}"/>.
   /// </summary>
   /// <typeparam name="T">The type of the object to serialize.</typeparam>
   /// <param name="value">The value to serialize.</param>
   /// <param name="writer">The buffer writer to write the payload into.</param>
   /// <returns>The number of bytes written.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Serialize<T>(scoped in T value, ref BufferWriter<byte> writer)
   {
      var context = new SerializationContext();
      SerializationContext.Current = context;

      try
      {
         return SerializerRegistry<T>.GetWrite()(ref writer, value);
      }
      finally
      {
         SerializationContext.Current = default;
         context.Dispose();
      }
   }

   /// <summary>
   /// Calculates the exact byte length required to serialize the specified value.
   /// </summary>
   /// <typeparam name="T">The type of the object to measure.</typeparam>
   /// <param name="value">The value to measure.</param>
   /// <returns>The required size in bytes.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int CalculateByteLength<T>(scoped in T value)
   {
      return SerializerRegistry<T>.GetCalculateByteLength()(value);
   }

   #endregion

   #region Deserialization

   /// <summary>
   /// Deserializes a value of type <typeparamref name="T"/> from a read-only sequence of bytes.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="sequence">The byte sequence to read from.</param>
   /// <returns>The deserialized value.</returns>
   /// <exception cref="InvalidOperationException">Thrown if deserialization fails.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Deserialize<T>(ReadOnlySequence<byte> sequence)
   {
      var reader = new SequenceReader<byte>(sequence);

      var context = new DeserializationContext();
      DeserializationContext.Current = context;

      try
      {
         if (!SerializerRegistry<T>.GetTryRead()(ref reader, out var value))
         {
            throw new InvalidOperationException($"Failed to deserialize type {typeof(T)} from sequence.");
         }
         return value;
      }
      finally
      {
         DeserializationContext.Current = default;
         context.Dispose();
      }
   }

   /// <summary>
   /// Deserializes a value of type <typeparamref name="T"/> from a read-only memory block of bytes.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="memory">The memory block to read from.</param>
   /// <returns>The deserialized value.</returns>
   /// <exception cref="InvalidOperationException">Thrown if deserialization fails.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Deserialize<T>(ReadOnlyMemory<byte> memory)
   {
      return Deserialize<T>(new ReadOnlySequence<byte>(memory));
   }

   /// <summary>
   /// Deserializes a value of type <typeparamref name="T"/> from a byte array.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="bytes">The byte array to read from.</param>
   /// <returns>The deserialized value.</returns>
   /// <exception cref="InvalidOperationException">Thrown if deserialization fails.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Deserialize<T>(byte[] bytes)
   {
      return Deserialize<T>(new ReadOnlySequence<byte>(bytes));
   }

   /// <summary>
   /// Deserializes a value of type <typeparamref name="T"/> from a read-only span of bytes.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="span">The span of bytes to read from.</param>
   /// <returns>The deserialized value.</returns>
   /// <exception cref="InvalidOperationException">Thrown if deserialization fails.</exception>
   public static T Deserialize<T>(ReadOnlySpan<byte> span)
   {
      if (span.IsEmpty)
      {
         return Deserialize<T>(ReadOnlySequence<byte>.Empty);
      }

      using var spanOwner = new MemoryOwner<byte>(span.Length);
      span.CopyTo(spanOwner);

      var sequence = new ReadOnlySequence<byte>(spanOwner.Memory);

      return Deserialize<T>(sequence);
   }

   /// <summary>
   /// Deserializes a value of type <typeparamref name="T"/> directly from an active <see cref="SequenceReader{byte}"/>.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="reader">The sequence reader to read from.</param>
   /// <returns>The deserialized value.</returns>
   /// <exception cref="InvalidOperationException">Thrown if deserialization fails.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Deserialize<T>(ref SequenceReader<byte> reader)
   {
      if (!SerializerRegistry<T>.GetTryRead()(ref reader, out var value))
      {
         throw new InvalidOperationException($"Failed to deserialize type {typeof(T)}.");
      }
      return value;
   }

   /// <summary>
   /// Tries to deserialize a value of type <typeparamref name="T"/> from a read-only sequence of bytes.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="sequence">The byte sequence to read from.</param>
   /// <param name="value">When this method returns, contains the deserialized value if successful, or default otherwise.</param>
   /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryDeserialize<T>(ReadOnlySequence<byte> sequence, [MaybeNullWhen(false)] out T value)
   {
      var reader = new SequenceReader<byte>(sequence);

      var context = new DeserializationContext();
      DeserializationContext.Current = context;

      try
      {
         return SerializerRegistry<T>.GetTryRead()(ref reader, out value);
      }
      finally
      {
         DeserializationContext.Current = default;
         context.Dispose();
      }
   }

   /// <summary>
   /// Tries to deserialize a value of type <typeparamref name="T"/> from a read-only memory block of bytes.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="memory">The memory block to read from.</param>
   /// <param name="value">When this method returns, contains the deserialized value if successful, or default otherwise.</param>
   /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryDeserialize<T>(ReadOnlyMemory<byte> memory, [MaybeNullWhen(false)] out T value)
   {
      return TryDeserialize<T>(new ReadOnlySequence<byte>(memory), out value);
   }

   /// <summary>
   /// Tries to deserialize a value of type <typeparamref name="T"/> from a byte array.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="bytes">The byte array to read from.</param>
   /// <param name="value">When this method returns, contains the deserialized value if successful, or default otherwise.</param>
   /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryDeserialize<T>(byte[] bytes, [MaybeNullWhen(false)] out T value)
   {
      return TryDeserialize<T>(new ReadOnlySequence<byte>(bytes), out value);
   }

   /// <summary>
   /// Tries to deserialize a value of type <typeparamref name="T"/> from a read-only span of bytes.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="span">The span of bytes to read from.</param>
   /// <param name="value">When this method returns, contains the deserialized value if successful, or default otherwise.</param>
   /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
   public static bool TryDeserialize<T>(ReadOnlySpan<byte> span, [MaybeNullWhen(false)] out T value)
   {
      if (span.IsEmpty)
      {
         return TryDeserialize(ReadOnlySequence<byte>.Empty, out value);
      }

      using var spanOwner = new MemoryOwner<byte>(span.Length);
      span.CopyTo(spanOwner);

      var sequence = new ReadOnlySequence<byte>(spanOwner.Memory);

      return TryDeserialize(sequence, out value);
   }

   /// <summary>
   /// Tries to deserialize a value of type <typeparamref name="T"/> directly from an active <see cref="SequenceReader{byte}"/>.
   /// </summary>
   /// <typeparam name="T">The type of the object to deserialize.</typeparam>
   /// <param name="reader">The sequence reader to read from.</param>
   /// <param name="value">When this method returns, contains the deserialized value if successful, or default otherwise.</param>
   /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryDeserialize<T>(ref SequenceReader<byte> reader, [MaybeNullWhen(false)] out T value)
   {
      return SerializerRegistry<T>.GetTryRead()(ref reader, out value);
   }

   #endregion
}
