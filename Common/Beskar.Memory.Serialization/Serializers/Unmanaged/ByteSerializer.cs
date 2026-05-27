using System.Buffers;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for bytes.
/// </summary>
public abstract class ByteSerializer : ISerializer<byte>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in byte value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(byte);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out byte value)
   {
      if (reader.UnreadSpan.Length >= sizeof(byte))
      {
         value = reader.UnreadSpan[0];
         reader.Advance(sizeof(byte));
         
         return true;
      }
      
      return reader.TryRead(out value);
   }

   public static int CalculateByteLength(scoped in byte value)
   {
      return sizeof(byte);
   }
}
