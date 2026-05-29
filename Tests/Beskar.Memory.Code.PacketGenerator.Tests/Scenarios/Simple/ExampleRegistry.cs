using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Beskar.Memory.Code.PacketGenerator.Attributes;
using Beskar.Memory.Code.PacketGenerator.Common;
using Beskar.Memory.Code.PacketGenerator.Interfaces;
using Beskar.Memory.Code.PacketGenerator.Models;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Code.PacketGenerator.Tests.Scenarios.Simple;

[PacketRegistry]
public sealed partial class ExampleRegistry : BaseJsonPacketRegistry<object>
{
   public override ValueTask<RoutePacketResult> RoutePacket(
      ref object state,
      scoped in ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
   {
      throw new System.NotImplementedException();
   }
}

[PacketRegistry]
public sealed partial class ExampleTwoRegistry : BasePacketRegistry<object>
{
   public override ValueTask<RoutePacketResult> RoutePacket(
      ref object state,
      scoped in ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
   {
      throw new System.NotImplementedException();
   }

   public override bool TryDeserialize<T>(ref SequenceReader<byte> reader, [MaybeNullWhen(false)] out T packet)
   {
      throw new System.NotImplementedException();
   }

   public override void Serialize<T>(ref BufferWriter<byte> writer, T packet)
   {
      throw new System.NotImplementedException();
   }
}

[Packet(typeof(ExampleRegistry))]
public sealed class PingPacket : IPacket
{
   public required string Name { get; set; }
}

public interface IClusterPacketPayload { }

public readonly struct ClusterPacket<TPacket> : IPacket
   where TPacket : IClusterPacketPayload
{
   public required TPacket Payload { get; init; }
}

[Packet(typeof(ExampleRegistry), Wrapper = typeof(ClusterPacket<>))]
public struct WrappedPingPayload : IClusterPacketPayload
{
   public required string Value { get; set; }
}
