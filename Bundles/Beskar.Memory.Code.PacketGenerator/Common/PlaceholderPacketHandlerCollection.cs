using System.Buffers;
using Beskar.Memory.Code.PacketGenerator.Enums;
using Beskar.Memory.Code.PacketGenerator.Interfaces;
using Beskar.Memory.Code.PacketGenerator.Models;

namespace Beskar.Memory.Code.PacketGenerator.Common;

public sealed class PlaceholderPacketHandlerCollection<TState, TPacket>(BasePacketRegistry<TState> registry) 
   : BasePacketHandlerCollection<TState, TPacket>(registry) where TPacket : IPacket
{
   public override void RegisterHandler(PacketHandler<TState, TPacket> handler)
   {
      throw new InvalidOperationException("This packet is not registered in this registry.");
   }

   public override ValueTask<RoutePacketResult> Handle(
      ref TState state, ref SequenceReader<byte> reader, CancellationToken cancellationToken)
   {
      throw new InvalidOperationException("This packet is not registered in this registry.");
   }
}
