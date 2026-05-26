namespace Beskar.Memory.Code.PacketGenerator.Enums;

public enum RoutePacketState : byte
{
   Success = 0,
   SuccessNoHandlers = 1,
   NotEnoughData = 2,
   UnknownPacket = 3,
   InvalidPacket = 4
}

public static class RoutePacketResultExtensions
{
   extension(RoutePacketState state)
   {
      public bool IsSuccess => state is RoutePacketState.Success or RoutePacketState.SuccessNoHandlers;
   }
}
