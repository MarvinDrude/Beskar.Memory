using Beskar.Memory.Code.PacketGenerator.Interfaces;

namespace Beskar.Memory.Code.PacketGenerator.Models;

public static class PacketMetadata<T>
   where T : IPacket
{
   public static int Identifier { get; set; }
}
