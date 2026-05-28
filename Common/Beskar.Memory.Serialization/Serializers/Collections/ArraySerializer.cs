using System.Buffers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Collections;

/// <summary>
/// Serializer for arrays using Varint length prefix.
/// </summary>
public abstract class ArraySerializer<T> : ISerializer<T[]?>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in T[]? value)
   {
      if (value is null)
      {
         return VarInteger.Write(ref writer, -1);
      }

      var writeElement = SerializerRegistry<T>.GetWrite();
      var written = VarInteger.Write(ref writer, value.Length);
      
      for (var i = 0; i < value.Length; i++)
      {
         written += writeElement(ref writer, value[i]);
      }

      return written;
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out T[]? value)
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

      var tryReadElement = SerializerRegistry<T>.GetTryRead();
      var array = new T[length];
      
      for (var i = 0; i < length; i++)
      {
         if (!tryReadElement(ref reader, out var element))
         {
            value = null;
            return false;
         }
         
         array[i] = element;
      }

      value = array;
      return true;
   }

   public static int CalculateByteLength(scoped in T[]? value)
   {
      if (value is null)
      {
         return VarInteger.CalculateByteLength(-1);
      }

      var calculateElement = SerializerRegistry<T>.GetCalculateByteLength();
      var length = VarInteger.CalculateByteLength(value.Length);
      
      for (var i = 0; i < value.Length; i++)
      {
         length += calculateElement(value[i]);
      }

      return length;
   }
}