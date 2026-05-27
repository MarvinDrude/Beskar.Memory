using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.System;

/// <summary>
/// Serializer for TimeOnly values.
/// </summary>
public abstract class TimeOnlySerializer : ISerializer<TimeOnly>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in TimeOnly value)
   {
      (value.Ticks).WriteLittleEndian(ref writer);
      return sizeof(long);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out TimeOnly value)
   {
      if (reader.UnreadSpan.Length >= sizeof(long))
      {
         var ticks = BinaryPrimitives.ReadInt64LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(long));
         value = new TimeOnly(ticks);
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out long t))
      {
         value = new TimeOnly(t);
         return true;
      }

      value = default;
      return false;
   }

   public static int CalculateByteLength(scoped in TimeOnly value)
   {
      return sizeof(long);
   }
}
