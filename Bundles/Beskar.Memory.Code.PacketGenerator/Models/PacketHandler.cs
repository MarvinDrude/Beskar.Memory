using Beskar.Memory.Code.PacketGenerator.Interfaces;

namespace Beskar.Memory.Code.PacketGenerator.Models;

public delegate ValueTask PacketHandler<TState, TPacket>(ref TState state, ref TPacket packet, CancellationToken cancellationToken)
   where TPacket : IPacket;
