using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.System;

/// <summary>
/// Serializer for TimeSpan values.
/// </summary>
public abstract class TimeSpanSerializer : ISerializer<TimeSpan>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in TimeSpan value)
   {
      (value.Ticks).WriteLittleEndian(ref writer);
      return sizeof(long);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out TimeSpan value)
   {
      if (reader.UnreadSpan.Length >= sizeof(long))
      {
         var ticks = BinaryPrimitives.ReadInt64LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(long));
         value = new TimeSpan(ticks);
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out long t))
      {
         value = new TimeSpan(t);
         return true;
      }

      value = default;
      return false;
   }

   public static int CalculateByteLength(scoped in TimeSpan value)
   {
      return sizeof(long);
   }
}
