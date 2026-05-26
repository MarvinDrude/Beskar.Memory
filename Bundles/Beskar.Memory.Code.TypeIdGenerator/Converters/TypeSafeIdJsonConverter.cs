using System.Text.Json;
using System.Text.Json.Serialization;
using Beskar.Memory.Code.TypeIdGenerator.Interfaces;

namespace Beskar.Memory.Code.TypeIdGenerator.Converters;

// maybe generate in the future for all
public sealed class TypeSafeIdJsonConverter<T, TUnderlying> : JsonConverter<T>
   where T : struct, ITypeSafeIdentifier<TUnderlying>, ISpanParsable<T>
   where TUnderlying : unmanaged
{
   public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      var str = reader.GetString();
      return str is not { Length: > 0 } 
         ? default 
         : T.Parse(str.AsSpan(), null);
   }

   public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      writer.WriteStringValue(value.Value.ToString());
   }

   public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      var str = reader.GetString();
      return str is null ? default : T.Parse(str, null);
   }

   public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      writer.WritePropertyName(value.Value.ToString() ?? throw new InvalidOperationException("Empty property name."));
   }
}