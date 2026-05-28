using System.Buffers;
using System.Runtime.CompilerServices;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 64-bit signed integers using Varint encoding.
/// </summary>
public abstract class Int64Serializer : ISerializer<long>
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Write(ref BufferWriter<byte> writer, scoped in long value)
   {
      return VarInteger.Write(ref writer, value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryRead(ref SequenceReader<byte> reader, out long value)
   {
      return VarInteger.TryRead(ref reader, out value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int CalculateByteLength(scoped in long value)
   {
      return VarInteger.CalculateByteLength(value);
   }
}
