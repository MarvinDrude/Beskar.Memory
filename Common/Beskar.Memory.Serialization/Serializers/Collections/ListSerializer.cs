using System.Buffers;
using System.Collections.Generic;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Interfaces;

namespace Beskar.Memory.Serialization.Serializers.Collections;

/// <summary>
/// Serializer for List values using Varint count prefix.
/// </summary>
public abstract class ListSerializer<T> : ISerializer<List<T>?>
{
   public static int Write(ref BufferWriter<byte> writer, scoped in List<T>? value)
   {
      if (value is null)
      {
         return VarInteger.Write(ref writer, -1);
      }

      var writeElement = SerializerRegistry<T>.GetWrite();
      var written = VarInteger.Write(ref writer, value.Count);
      
      for (var i = 0; i < value.Count; i++)
      {
         written += writeElement(ref writer, value[i]);
      }

      return written;
   }

   public static bool TryRead(ref SequenceReader<byte> reader, out List<T>? value)
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

      CollectionValidation.ValidateLength(length);

      var tryReadElement = SerializerRegistry<T>.GetTryRead();
      var list = new List<T>(length);

      for (var i = 0; i < length; i++)
      {
         if (!tryReadElement(ref reader, out var element))
         {
            value = null;
            return false;
         }
         
         list.Add(element);
      }

      value = list;
      return true;
   }

   public static int CalculateByteLength(scoped in List<T>? value)
   {
      if (value is null)
      {
         return VarInteger.CalculateByteLength(-1);
      }

      var calculateElement = SerializerRegistry<T>.GetCalculateByteLength();
      var length = VarInteger.CalculateByteLength(value.Count);
      
      for (var i = 0; i < value.Count; i++)
      {
         length += calculateElement(value[i]);
      }

      return length;
   }
}
