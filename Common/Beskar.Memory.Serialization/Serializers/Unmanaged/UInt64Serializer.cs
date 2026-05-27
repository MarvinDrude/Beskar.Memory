using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 64-bit unsigned integers.
/// </summary>
public abstract class UInt64Serializer : ISerializer<ulong>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in ulong value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(ulong);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out ulong value)
   {
      if (reader.UnreadSpan.Length >= sizeof(ulong))
      {
         value = BinaryPrimitives.ReadUInt64LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(ulong));
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out long l))
      {
         value = (ulong)l;
         return true;
      }

      value = 0;
      return false;
   }

   public static int CalculateByteLength(scoped in ulong value)
   {
      return sizeof(ulong);
   }
}
