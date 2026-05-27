using System.Buffers;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for booleans.
/// </summary>
public abstract class BooleanSerializer : ISerializer<bool>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in bool value)
   {
      ((byte)(value ? 1 : 0)).WriteLittleEndian(ref writer);
      return sizeof(bool);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out bool value)
   {
      if (reader.UnreadSpan.Length >= sizeof(byte))
      {
         value = reader.UnreadSpan[0] != 0;
         reader.Advance(sizeof(byte));
         
         return true;
      }
      
      if (reader.TryRead(out var b))
      {
         value = b != 0;
         return true;
      }

      value = false;
      return false;
   }

   public static int CalculateByteLength(scoped in bool value)
   {
      return sizeof(byte);
   }
}
