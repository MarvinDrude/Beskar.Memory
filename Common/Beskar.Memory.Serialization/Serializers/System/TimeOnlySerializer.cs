using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.System;

/// <summary>
/// Serializer for TimeOnly values using Varint encoding.
/// </summary>
public abstract class TimeOnlySerializer : ISerializer<TimeOnly>
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Write(ref BufferWriter<byte> writer, scoped in TimeOnly value)
   {
      return VarInteger.Write(ref writer, value.Ticks);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryRead(ref SequenceReader<byte> reader, out TimeOnly value)
   {
      if (VarInteger.TryRead(ref reader, out long ticks))
      {
         value = new TimeOnly(ticks);
         return true;
      }

      value = default;
      return false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int CalculateByteLength(scoped in TimeOnly value)
   {
      return VarInteger.CalculateByteLength(value.Ticks);
   }
}
