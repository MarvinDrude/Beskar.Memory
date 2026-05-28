using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.System;

/// <summary>
/// Serializer for DateOnly values using Varint encoding.
/// </summary>
public abstract class DateOnlySerializer : ISerializer<DateOnly>
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Write(ref BufferWriter<byte> writer, scoped in DateOnly value)
   {
      return VarInteger.Write(ref writer, value.DayNumber);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryRead(ref SequenceReader<byte> reader, out DateOnly value)
   {
      if (VarInteger.TryRead(ref reader, out int dayNumber))
      {
         value = DateOnly.FromDayNumber(dayNumber);
         return true;
      }

      value = default;
      return false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int CalculateByteLength(scoped in DateOnly value)
   {
      return VarInteger.CalculateByteLength(value.DayNumber);
   }
}
