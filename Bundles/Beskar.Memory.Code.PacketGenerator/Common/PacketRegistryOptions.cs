namespace Beskar.Memory.Code.PacketGenerator.Common;

public sealed class PacketRegistryOptions
{
   /// <summary>
   /// Whether more than one handler should be run in parallel for the same packet.
   /// </summary>
   public bool RunHandlersInParallel { get; set; } = true;
}
