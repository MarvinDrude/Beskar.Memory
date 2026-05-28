using System.Buffers;
using System.Runtime.CompilerServices;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Unmanaged;

/// <summary>
/// Serializer for 64-bit unsigned integers using Varint encoding.
/// </summary>
public abstract class UInt64Serializer : ISerializer<ulong>
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Write(ref BufferWriter<byte> writer, scoped in ulong value)
   {
      return VarInteger.Write(ref writer, value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryRead(ref SequenceReader<byte> reader, out ulong value)
   {
      return VarInteger.TryRead(ref reader, out value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int CalculateByteLength(scoped in ulong value)
   {
      return VarInteger.CalculateByteLength(value);
   }
}
