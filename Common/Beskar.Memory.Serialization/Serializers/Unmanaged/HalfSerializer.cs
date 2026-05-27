using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Beskar.Memory.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Extensions;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for half-precision floating point numbers.
/// </summary>
public abstract class HalfSerializer : ISerializer<Half>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in Half value)
   {
      (value).WriteLittleEndian(ref writer);
      return Unsafe.SizeOf<Half>();
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out Half value)
   {
      if (reader.UnreadSpan.Length >= Unsafe.SizeOf<Half>())
      {
         value = BinaryPrimitives.ReadHalfLittleEndian(reader.UnreadSpan);
         reader.Advance(Unsafe.SizeOf<Half>());
         
         return true;
      }
      
      if (reader.TryReadLittleEndian(out short s))
      {
         value = BitConverter.Int16BitsToHalf(s);
         return true;
      }

      value = default;
      return false;
   }

   public static int CalculateByteLength(scoped in Half value)
   {
      return Unsafe.SizeOf<Half>();
   }
}
