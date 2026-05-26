using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Code.PacketGenerator.Attributes;
using Beskar.Memory.Code.PacketGenerator.Common;
using Beskar.Memory.Code.PacketGenerator.Enums;
using Beskar.Memory.Code.PacketGenerator.Interfaces;
using Beskar.Memory.Code.PacketGenerator.Models;

namespace Beskar.Memory.Code.PacketGenerator.Internal;

[PacketRegistry]
internal sealed class ExamplePacketRegistry : BaseJsonPacketRegistry<object>
{
   private readonly IPacketHandlerCollection<object>[] _handlers;

   public ExamplePacketRegistry(PacketRegistryOptions? options = null)
      : base(registryOptions: options)
   {
      _handlers =
      [
         new PingPacketHandlerCollection(this),
         new PongPacketHandlerCollection(this)
      ];
   }

   public bool RegisterHandler<TPacket>(PacketHandler<object, TPacket> handler)
      where TPacket : IPacket
   {
      var packetId = PacketMetadata<TPacket>.Identifier;
      var handlerCollection = _handlers[packetId];

      if (handlerCollection is not IPacketHandlerCollection<object, TPacket> handlerCollectionTyped) 
         return false;
      
      handlerCollectionTyped.RegisterHandler(handler);
      return true;
   }

   public override ValueTask<RoutePacketResult> RoutePacket(
      ref object state,
      scoped in ReadOnlySequence<byte> sequence,
      CancellationToken cancellationToken = default)
   {
      var reader = new SequenceReader<byte>(sequence);
      if (!reader.TryReadLittleEndian(out int packetId))
      {
         return ValueTask.FromResult(RoutePacketResult.NotEnoughData);
      }

      if (packetId < 0 || packetId >= _handlers.Length)
      {
         return ValueTask.FromResult(RoutePacketResult.UnknownPacket);
      }
      
      ref var arrayPointer = ref MemoryMarshal.GetArrayDataReference(_handlers);
      var handlerCollection = Unsafe.Add(ref arrayPointer, (nint)packetId);
      
      return handlerCollection.Handle(ref state, ref reader, cancellationToken);
   }
}

file sealed class PingPacketHandlerCollection(ExamplePacketRegistry registry)
   : BasePacketHandlerCollection<object, PingPacket>(registry);

file sealed class PongPacketHandlerCollection(ExamplePacketRegistry registry)
   : BasePacketHandlerCollection<object, PongPacket>(registry);
