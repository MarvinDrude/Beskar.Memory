using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common;

public static class TypedConstantArrayExtensions
{
   extension(TypedConstant constant)
   {
      public bool IsArray => constant.Kind is TypedConstantKind.Array;

      public string?[] StringArrayValues => constant.GetStringValuesOrDefault();
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string?[] GetStringValuesOrDefault(string? defaultValue = null)
      {
         return constant.MapArray(x => x.GetStringValueOrDefault(defaultValue));
      }
      
      public bool[] BoolArrayValues => constant.GetBoolValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool[] GetBoolValuesOrDefault(bool defaultValue = false)
      {
         return constant.MapArray(x => x.GetBoolValueOrDefault(defaultValue));
      }
      
      public ITypeSymbol?[] TypeArrayValues => constant.GetTypeValues();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ITypeSymbol?[] GetTypeValues()
      {
         return constant.MapArray(x => x.GetTypeValue());
      }
      
      public char[] CharArrayValues => constant.GetCharValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public char[] GetCharValuesOrDefault(char defaultValue = '\0')
      {
         return constant.MapArray(x => x.GetCharValueOrDefault(defaultValue));
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public T[] GetNumberValuesOrDefault<T>(T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return constant.MapArray(x => x.GetNumberValueOrDefault(defaultValue));
      }

      public int[] IntArrayValues => constant.GetIntValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int[] GetIntValuesOrDefault(int defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }
      
      public long[] LongArrayValues => constant.GetLongValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public long[] GetLongValuesOrDefault(long defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      public float[] FloatArrayValues => constant.GetFloatValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public float[] GetFloatValuesOrDefault(float defaultValue = 0.0f)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      public double[] DoubleArrayValues => constant.GetDoubleValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public double[] GetDoubleValuesOrDefault(double defaultValue = 0.0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      public decimal[] DecimalArrayValues => constant.GetDecimalValuesOrDefault();
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public decimal[] GetDecimalValuesOrDefault(decimal defaultValue = 0.0m)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }
      
      public uint[] UIntArrayValues => constant.GetUIntValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public uint[] GetUIntValuesOrDefault(uint defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      public ulong[] ULongArrayValues => constant.GetULongValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ulong[] GetULongValuesOrDefault(ulong defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }
      
      public string?[] EnumFullNameArrayValues => constant.GetEnumFullNameValuesOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string?[] GetEnumFullNameValuesOrDefault(string? defaultValue = null)
      {
         return constant.MapArray(x => x.GetEnumFullNameValueOrDefault(defaultValue));
      }
      
      public IFieldSymbol?[] EnumFieldArrayValues => constant.GetEnumFieldValues();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IFieldSymbol?[] GetEnumFieldValues()
      {
         return constant.MapArray(x => x.GetEnumFieldSymbol());
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private T[] MapArray<T>(Func<TypedConstant, T> selector)
      {
         return constant is { IsArray: true, IsNull: false }
            ? constant.Values.Select(selector).ToArray()
            : [];
      }
   }
}

