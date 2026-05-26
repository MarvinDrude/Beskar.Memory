using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Beskar.Memory.Code.PacketGenerator.Attributes;
using Beskar.Memory.Code.PacketGenerator.Common;
using Beskar.Memory.Code.PacketGenerator.Interfaces;
using Beskar.Memory.Code.PacketGenerator.Models;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Code.PacketGenerator.Tests.Scenarios.Stateful;

public sealed class ClientState
{
   public int ClientId { get; set; }
}

[PacketRegistry<ClientState>]
public sealed partial class StatefulRegistry : BasePacketRegistry<ClientState>
{
   public override ValueTask<RoutePacketResult> RoutePacket(
      ref ClientState state,
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

[Packet(typeof(StatefulRegistry))]
public sealed class StatefulPacket : IPacket
{
   public required string Data { get; set; }
}
