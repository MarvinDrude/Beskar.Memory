using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for double-precision floating point numbers.
/// </summary>
public abstract class DoubleSerializer : ISerializer<double>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in double value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(double);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out double value)
   {
      if (reader.UnreadSpan.Length >= sizeof(double))
      {
         value = BinaryPrimitives.ReadDoubleLittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(double));
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out long l))
      {
         value = BitConverter.Int64BitsToDouble(l);
         return true;
      }

      value = 0;
      return false;
   }

   public static int CalculateByteLength(scoped in double value)
   {
      return sizeof(double);
   }
}
