using Beskar.Memory.Code.PacketGenerator.Attributes;
using Beskar.Memory.Code.PacketGenerator.Interfaces;
using Beskar.Memory.Code.PacketGenerator.Tests.Scenarios.Simple;

namespace Beskar.Test
{
   [Packet(typeof(ExampleRegistry), typeof(ExampleTwoRegistry))]
   public struct SeparatePacket : IPacket
   {
      
   }
}

[Packet(typeof(ExampleTwoRegistry))]
public struct SeparateGlobalPacket : IPacket
{
   
}
