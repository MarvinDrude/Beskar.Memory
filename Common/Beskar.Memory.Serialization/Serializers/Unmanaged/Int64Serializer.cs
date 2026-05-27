using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 64-bit signed integers.
/// </summary>
public abstract class Int64Serializer : ISerializer<long>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in long value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(long);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out long value)
   {
      if (reader.UnreadSpan.Length >= sizeof(long))
      {
         value = BinaryPrimitives.ReadInt64LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(long));
         
         return true;
      }
      
      return reader.TryReadLittleEndian(out value);
   }

   public static int CalculateByteLength(scoped in long value)
   {
      return sizeof(long);
   }
}
