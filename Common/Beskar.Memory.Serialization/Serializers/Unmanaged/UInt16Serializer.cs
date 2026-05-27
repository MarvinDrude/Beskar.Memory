using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 16-bit unsigned integers.
/// </summary>
public abstract class UInt16Serializer : ISerializer<ushort>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in ushort value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(ushort);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out ushort value)
   {
      if (reader.UnreadSpan.Length >= sizeof(ushort))
      {
         value = BinaryPrimitives.ReadUInt16LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(ushort));
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out short s))
      {
         value = (ushort)s;
         return true;
      }

      value = 0;
      return false;
   }

   public static int CalculateByteLength(scoped in ushort value)
   {
      return sizeof(ushort);
   }
}
