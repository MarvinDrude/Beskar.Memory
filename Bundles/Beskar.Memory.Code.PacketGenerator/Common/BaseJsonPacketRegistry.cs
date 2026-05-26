using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Code.PacketGenerator.Common;

public abstract class BaseJsonPacketRegistry<TState>(
   JsonSerializerContext? context = null,
   JsonSerializerOptions? options = null,
   PacketRegistryOptions? registryOptions = null) 
   : BasePacketRegistry<TState>(registryOptions)
{
   private readonly JsonSerializerContext? _context = context;
   private readonly JsonSerializerOptions _options = options ?? _defaultOptions;
   
   public override bool TryDeserialize<T>(
      ref SequenceReader<byte> sequenceReader, 
      [MaybeNullWhen(false)] out T packet)
   {
      try
      {
         var sequence = sequenceReader.UnreadSequence;
         var typeInfo = _context?.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>;
         
         var reader = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
         var ob = typeInfo is not null 
            ? JsonSerializer.Deserialize(ref reader, typeInfo)
            : JsonSerializer.Deserialize<T>(ref reader, _options);

         if (ob is not null)
         {
            sequenceReader.Advance(reader.BytesConsumed);
         }
         
         packet = ob;
         return ob is not null;
      }
      catch (Exception)
      {
         packet = default;
         return false;
      }
   }

   public override void Serialize<T>(ref BufferWriter<byte> writer, T packet)
   {
      var typeInfo = _context?.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>;
      var arrayWriter = new ArrayBufferWriter<byte>(1024); // double heap is ok for now

      using var jsonWriter = new Utf8JsonWriter(arrayWriter, new JsonWriterOptions()
      {
         Indented = false,
         SkipValidation = true
      });

      if (typeInfo is not null)
      {
         JsonSerializer.Serialize(jsonWriter, packet, typeInfo);
      }
      else
      {
         JsonSerializer.Serialize(jsonWriter, packet, _options);
      }
      
      jsonWriter.Flush();
      writer.Write(arrayWriter.WrittenSpan);
   }

   private static readonly JsonSerializerOptions _defaultOptions = new()
   {
      WriteIndented = false,
      AllowTrailingCommas = true,
   };
}
