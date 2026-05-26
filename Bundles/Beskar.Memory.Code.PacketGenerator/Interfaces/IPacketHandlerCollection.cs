using System.Buffers;
using Beskar.Memory.Code.PacketGenerator.Enums;
using Beskar.Memory.Code.PacketGenerator.Models;

namespace Beskar.Memory.Code.PacketGenerator.Interfaces;

public interface IPacketHandlerCollection<TState>
{
   public int HandlerCount { get; }
   
   public ValueTask<RoutePacketResult> Handle(ref TState state, ref SequenceReader<byte> reader, CancellationToken cancellationToken);
}

public interface IPacketHandlerCollection<TState, TPacket> : IPacketHandlerCollection<TState>
   where TPacket : IPacket
{
   public void RegisterHandler(PacketHandler<TState, TPacket> handler);
}
