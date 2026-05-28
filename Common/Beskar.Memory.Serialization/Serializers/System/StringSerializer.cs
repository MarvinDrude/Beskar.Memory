using System;
using System.Buffers;
using System.Text;
using Beskar.Memory.Writers;
using Beskar.Memory.Owners;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.System;

/// <summary>
/// Serializer for string using Varint length prefix.
/// </summary>
public abstract class StringSerializer : ISerializer<string?>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in string? value)
   {
      if (value is null)
      {
         return VarInteger.Write(ref writer, -1);
      }

      var byteCount = Encoding.UTF8.GetByteCount(value);
      var headerLen = VarInteger.Write(ref writer, byteCount);
      
      var span = writer.AcquireSpan(byteCount);
      Encoding.UTF8.GetBytes(value, span);

      return headerLen + byteCount;
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out string? value)
   {
      if (!VarInteger.TryRead(ref reader, out int length))
      {
         value = null;
         return false;
      }

      if (length < 0)
      {
         value = null;
         return true;
      }

      if (reader.Remaining < length)
      {
         value = null;
         return false;
      }

      if (reader.UnreadSpan.Length >= length)
      {
         value = Encoding.UTF8.GetString(reader.UnreadSpan[..length]);
         reader.Advance(length);

         return true;
      }

      using var owner = length <= 256
         ? new SpanOwner<byte>(stackalloc byte[length])
         : new SpanOwner<byte>(length);
      var stringBytes = owner.Span;

      reader.UnreadSequence.Slice(0, length).CopyTo(stringBytes);

      value = Encoding.UTF8.GetString(stringBytes);
      reader.Advance(length);

      return true;
   }

   public static int CalculateByteLength(scoped in string? value)
   {
      if (value is null) return VarInteger.CalculateByteLength(-1);
      var byteCount = Encoding.UTF8.GetByteCount(value);
      return VarInteger.CalculateByteLength(byteCount) + byteCount;
   }
}
