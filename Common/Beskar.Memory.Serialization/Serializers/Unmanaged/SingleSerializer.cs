using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for single-precision floating point numbers.
/// </summary>
public abstract class SingleSerializer : ISerializer<float>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in float value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(float);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out float value)
   {
      if (reader.UnreadSpan.Length >= sizeof(float))
      {
         value = BinaryPrimitives.ReadSingleLittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(float));
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out int i))
      {
         value = BitConverter.Int32BitsToSingle(i);
         return true;
      }

      value = 0f;
      return false;
   }

   public static int CalculateByteLength(scoped in float value)
   {
      return sizeof(float);
   }
}
