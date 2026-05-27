using System.Buffers;
using System.Buffers.Binary;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 32-bit integers.
/// </summary>
public abstract class Int32Serializer : ISerializer<int>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in int value)
   {
      (value).WriteLittleEndian(ref writer);
      return sizeof(int);
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out int value)
   {
      if (reader.UnreadSpan.Length >= sizeof(int))
      {
         value = BinaryPrimitives.ReadInt32LittleEndian(reader.UnreadSpan);
         reader.Advance(sizeof(int));
         
         return true;
      }
      
      return reader.TryReadLittleEndian(out value);
   }

   public static int CalculateByteLength(scoped in int value)
   {
      return sizeof(int);
   }
}