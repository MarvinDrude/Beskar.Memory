using Beskar.Memory.Code.PacketGenerator.Enums;

namespace Beskar.Memory.Code.PacketGenerator.Models;

public readonly struct RoutePacketResult
{
   public RoutePacketState State { get; init; }
   
   public long ConsumedBytes { get; init; }
   
   public static RoutePacketResult Success(long consumedBytes) => new ()
   {
      State = RoutePacketState.Success,
      ConsumedBytes = consumedBytes
   };

   public static RoutePacketResult SuccessNoHandlers(long consumedBytes) => new()
   {
      State = RoutePacketState.SuccessNoHandlers,
      ConsumedBytes = consumedBytes
   };

   public static RoutePacketResult InvalidPacket(long consumedBytes) => new()
   {
      State = RoutePacketState.InvalidPacket,
      ConsumedBytes = 0
   };

   public static readonly RoutePacketResult UnknownPacket = new ()
   {
      State = RoutePacketState.InvalidPacket,
      ConsumedBytes = 0
   };

   public static readonly RoutePacketResult NotEnoughData = new()
   {
      State = RoutePacketState.NotEnoughData,
      ConsumedBytes = 0
   };
}
