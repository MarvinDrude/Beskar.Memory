using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 32-bit unsigned integers.
/// </summary>
public abstract class UInt32Serializer : ISerializer<uint>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in uint value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(uint);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out uint value)
   {
      if (reader.UnreadSpan.Length >= sizeof(uint))
      {
         value = BinaryPrimitives.ReadUInt32LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(uint));
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out int i))
      {
         value = (uint)i;
         return true;
      }

      value = 0;
      return false;
   }

   public static int CalculateByteLength(scoped in uint value)
   {
      return sizeof(uint);
   }
}
