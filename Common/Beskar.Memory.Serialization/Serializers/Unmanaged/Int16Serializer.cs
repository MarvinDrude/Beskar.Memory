using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 16-bit signed integers.
/// </summary>
public abstract class Int16Serializer : ISerializer<short>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in short value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(short);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out short value)
   {
      if (reader.UnreadSpan.Length >= sizeof(short))
      {
         value = BinaryPrimitives.ReadInt16LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(short));
         
         return true;
      }
      
      return reader.TryReadLittleEndian(out value);
   }

   public static int CalculateByteLength(scoped in short value)
   {
      return sizeof(short);
   }
}
