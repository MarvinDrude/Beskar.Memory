#pragma warning disable CA2255

using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Beskar.Memory.Serialization.Serializers.Collections;
using Beskar.Memory.Serialization.Serializers.System;
using Beskar.Memory.Serialization.Serializers.Unmanaged;

namespace Beskar.Memory.Serialization.Serializers;

/// <summary>
/// Registers the default serializers for common types.
/// </summary>
internal static class DefaultSerializerRegistration
{
   [ModuleInitializer]
   internal static void RegisterDefaults()
   {
      // Unmanaged
      SerializerRegistry<bool>.Register<BooleanSerializer>();
      SerializerRegistry<bool?>.Register<NullableSerializer<bool>>();
      SerializerRegistry<byte>.Register<ByteSerializer>();
      SerializerRegistry<byte?>.Register<NullableSerializer<byte>>();
      SerializerRegistry<char>.Register<CharSerializer>();
      SerializerRegistry<char?>.Register<NullableSerializer<char>>();
      SerializerRegistry<decimal>.Register<DecimalSerializer>();
      SerializerRegistry<decimal?>.Register<NullableSerializer<decimal>>();
      SerializerRegistry<double>.Register<DoubleSerializer>();
      SerializerRegistry<double?>.Register<NullableSerializer<double>>();
      SerializerRegistry<Half>.Register<HalfSerializer>();
      SerializerRegistry<Half?>.Register<NullableSerializer<Half>>();
      SerializerRegistry<short>.Register<Int16Serializer>();
      SerializerRegistry<short?>.Register<NullableSerializer<short>>();
      SerializerRegistry<int>.Register<Int32Serializer>();
      SerializerRegistry<int?>.Register<NullableSerializer<int>>();
      SerializerRegistry<long>.Register<Int64Serializer>();
      SerializerRegistry<long?>.Register<NullableSerializer<long>>();
      SerializerRegistry<sbyte>.Register<SByteSerializer>();
      SerializerRegistry<sbyte?>.Register<NullableSerializer<sbyte>>();
      SerializerRegistry<float>.Register<SingleSerializer>();
      SerializerRegistry<float?>.Register<NullableSerializer<float>>();
      SerializerRegistry<ushort>.Register<UInt16Serializer>();
      SerializerRegistry<ushort?>.Register<NullableSerializer<ushort>>();
      SerializerRegistry<uint>.Register<UInt32Serializer>();
      SerializerRegistry<uint?>.Register<NullableSerializer<uint>>();
      SerializerRegistry<ulong>.Register<UInt64Serializer>();
      SerializerRegistry<ulong?>.Register<NullableSerializer<ulong>>();

      // System
      SerializerRegistry<string?>.Register<StringSerializer>();
      SerializerRegistry<TimeSpan>.Register<TimeSpanSerializer>();
      SerializerRegistry<TimeSpan?>.Register<NullableSerializer<TimeSpan>>();
      SerializerRegistry<DateTime>.Register<DateTimeSerializer>();
      SerializerRegistry<DateTime?>.Register<NullableSerializer<DateTime>>();
      SerializerRegistry<DateTimeOffset>.Register<DateTimeOffsetSerializer>();
      SerializerRegistry<DateTimeOffset?>.Register<NullableSerializer<DateTimeOffset>>();
      SerializerRegistry<Guid>.Register<GuidSerializer>();
      SerializerRegistry<Guid?>.Register<NullableSerializer<Guid>>();
      SerializerRegistry<Uri?>.Register<UriSerializer>();
      SerializerRegistry<TimeOnly>.Register<TimeOnlySerializer>();
      SerializerRegistry<TimeOnly?>.Register<NullableSerializer<TimeOnly>>();
      SerializerRegistry<DateOnly>.Register<DateOnlySerializer>();
      SerializerRegistry<DateOnly?>.Register<NullableSerializer<DateOnly>>();
      SerializerRegistry<BigInteger>.Register<BigIntegerSerializer>();
      SerializerRegistry<BigInteger?>.Register<NullableSerializer<BigInteger>>();
      SerializerRegistry<Complex>.Register<ComplexSerializer>();
      SerializerRegistry<Complex?>.Register<NullableSerializer<Complex>>();
      SerializerRegistry<StringBuilder?>.Register<StringBuilderSerializer>();

      // Collections
      SerializerRegistry<BitArray?>.Register<BitArraySerializer>();
   }
}
