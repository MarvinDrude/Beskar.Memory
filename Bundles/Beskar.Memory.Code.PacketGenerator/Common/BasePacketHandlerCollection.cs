using System.Buffers;
using System.Collections.Concurrent;
using Beskar.Memory.Code.PacketGenerator.Enums;
using Beskar.Memory.Code.PacketGenerator.Interfaces;
using Beskar.Memory.Code.PacketGenerator.Models;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Code.PacketGenerator.Common;

public abstract class BasePacketHandlerCollection<TState, TPacket>(
   BasePacketRegistry<TState> registry)
   : IPacketHandlerCollection<TState, TPacket>
   where TPacket : IPacket
{
   public int HandlerCount { get; private set; }

   private readonly BasePacketRegistry<TState> _registry = registry;
   private readonly ConcurrentStack<PacketHandler<TState, TPacket>> _handlers = [];

   public virtual void RegisterHandler(PacketHandler<TState, TPacket> handler)
   {
      _handlers.Push(handler);
      HandlerCount++;
   }

   public virtual ValueTask<RoutePacketResult> Handle(
      ref TState state, ref SequenceReader<byte> reader, CancellationToken cancellationToken)
   {
      if (!_registry.TryDeserialize<TPacket>(ref reader, out var packet))
      {
         return ValueTask.FromResult(RoutePacketResult.InvalidPacket(reader.Consumed));
      }

      if (HandlerCount == 0)
      {
         return ValueTask.FromResult(RoutePacketResult.SuccessNoHandlers(reader.Consumed));
      }

      if (HandlerCount != 1 || !_handlers.TryPeek(out var handler))
         return _registry.Options.RunHandlersInParallel 
            ? InvokeParallelAsync(state, packet, reader.Consumed, cancellationToken)
            : InvokeIterateAsync(state, packet, reader.Consumed, cancellationToken);
      
      var singleTask = handler.Invoke(ref state, ref packet, cancellationToken);
      return singleTask.IsCompletedSuccessfully
         ? ValueTask.FromResult(RoutePacketResult.Success(reader.Consumed))
         : InvokeSingleAsync(singleTask, reader.Consumed);
   }
   
   private async ValueTask<RoutePacketResult> InvokeSingleAsync(ValueTask task, long consumed)
   {
      await task;
      return RoutePacketResult.Success(consumed);
   }

   private async ValueTask<RoutePacketResult> InvokeIterateAsync(
      TState state, TPacket packet, long consumed, CancellationToken cancellationToken)
   {
      using var enumerator = _handlers.GetEnumerator();
      while (enumerator.MoveNext())
      {
         await enumerator.Current.Invoke(ref state, ref packet, cancellationToken);
      }
      
      return RoutePacketResult.Success(consumed);
   }

   private async ValueTask<RoutePacketResult> InvokeParallelAsync(
      TState state, TPacket packet, long consumed, CancellationToken cancellationToken)
   {
      using var builder = new ArrayBuilder<Task>(_handlers.Count);
      
      using var enumerator = _handlers.GetEnumerator();
      while (enumerator.MoveNext())
      {
         var valueTask = enumerator.Current.Invoke(ref state, ref packet, cancellationToken);
         if (!valueTask.IsCompletedSuccessfully)
         {
            builder.Add(valueTask.AsTask());
         }
      }
      
      await Task.WhenAll(builder.WrittenSpan);
      return RoutePacketResult.Success(consumed);
   }
}
