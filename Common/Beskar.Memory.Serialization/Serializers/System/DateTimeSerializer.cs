using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.System;

/// <summary>
/// Serializer for DateTime values using Varint encoding.
/// </summary>
public abstract class DateTimeSerializer : ISerializer<DateTime>
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Write(ref BufferWriter<byte> writer, scoped in DateTime value)
   {
      return VarInteger.Write(ref writer, value.ToBinary());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool TryRead(ref SequenceReader<byte> reader, out DateTime value)
   {
      if (VarInteger.TryRead(ref reader, out long binary))
      {
         value = DateTime.FromBinary(binary);
         return true;
      }

      value = default;
      return false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int CalculateByteLength(scoped in DateTime value)
   {
      return VarInteger.CalculateByteLength(value.ToBinary());
   }
}
