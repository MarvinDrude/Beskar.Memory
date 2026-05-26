using Beskar.Memory.Code.PacketGenerator.Attributes;
using Beskar.Memory.Code.PacketGenerator.Interfaces;

namespace Beskar.Memory.Code.PacketGenerator.Internal;

[Packet(typeof(ExamplePacketRegistry))]
internal sealed class PingPacket : IPacket
{
   public required string Name { get; set; }
}

[Packet(typeof(ExamplePacketRegistry))]
internal struct PongPacket : IPacket
{
   public required int Number { get; set; }
}
