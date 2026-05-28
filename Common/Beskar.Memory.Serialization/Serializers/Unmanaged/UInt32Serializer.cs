using System.Buffers;
using System.Runtime.CompilerServices;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 32-bit unsigned integers using Varint encoding.
/// </summary>
public abstract class UInt32Serializer : ISerializer<uint>
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Write(ref BufferWriter<byte> writer, scoped in uint value)
   {
      return VarInteger.Write(ref writer, value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryRead(ref SequenceReader<byte> reader, out uint value)
   {
      return VarInteger.TryRead(ref reader, out value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int CalculateByteLength(scoped in uint value)
   {
      return VarInteger.CalculateByteLength(value);
   }
}
