using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Beskar.Memory.Code.PacketGenerator.Interfaces;
using Beskar.Memory.Code.PacketGenerator.Models;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;

namespace Beskar.Memory.Code.PacketGenerator.Common;

public abstract class BasePacketRegistry<TState>(PacketRegistryOptions? options = null)
{
   public PacketRegistryOptions Options { get; } = options ?? new PacketRegistryOptions();
   
   public abstract ValueTask<RoutePacketResult> RoutePacket(
      ref TState state,
      scoped in ReadOnlySequence<byte> sequence,
      CancellationToken cancellationToken = default);
   
   public abstract bool TryDeserialize<T>(
      ref SequenceReader<byte> reader, 
      [MaybeNullWhen(false)] out T packet)
      where T : IPacket;
   
   public abstract void Serialize<T>(
      ref BufferWriter<byte> writer, T packet)
      where T : IPacket;
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public byte[] Serialize<T>(T packet)
      where T : IPacket
   {
      var writer = new BufferWriter<byte>(512);
      try
      {
         Serialize(ref writer, packet);
         return writer.WrittenSpan.ToArray();
      }
      finally
      {
         writer.Dispose();
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void SerializeWithHeader<T>(
      ref BufferWriter<byte> writer, T packet)
      where T : IPacket
   {
      var packetId = PacketMetadata<T>.Identifier;
      packetId.WriteLittleEndian(ref writer);
      
      Serialize(ref writer, packet);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public byte[] SerializeWithHeader<T>(T packet)
      where T : IPacket
   {
      var packetId = PacketMetadata<T>.Identifier;
      var writer = new BufferWriter<byte>(512);
      try
      {
         packetId.WriteLittleEndian(ref writer);
         Serialize(ref writer, packet);
         
         return writer.WrittenSpan.ToArray();
      }
      finally
      {
         writer.Dispose();
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTask<RoutePacketResult> RoutePacket(
      ref TState state, byte[] bytes, CancellationToken cancellationToken = default)
   {
      var sequence = new ReadOnlySequence<byte>(bytes);
      return RoutePacket(ref state, sequence, cancellationToken);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTask<RoutePacketResult> RoutePacket(
      ref TState state, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
   {
      var sequence = new ReadOnlySequence<byte>(memory);
      return RoutePacket(ref state, sequence, cancellationToken);
   }
}
