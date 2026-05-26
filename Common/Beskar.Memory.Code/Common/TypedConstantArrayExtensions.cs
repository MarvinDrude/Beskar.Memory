using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common;

/// <summary>
/// Provides extension members for <see cref="TypedConstant"/> to retrieve array values mapped to their corresponding C# primitive or object types.
/// </summary>
public static class TypedConstantArrayExtensions
{
   extension(TypedConstant constant)
   {
      /// <summary>
      /// Gets a value indicating whether the typed constant represents an array.
      /// </summary>
      public bool IsArray => constant.Kind is TypedConstantKind.Array;

      /// <summary>
      /// Gets the string array values of the typed constant, using default formatting.
      /// </summary>
      public string?[] StringArrayValues => constant.GetStringValuesOrDefault();
      
      /// <summary>
      /// Retrieves the string array values, or a default value for null elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if a string element is null.</param>
      /// <returns>An array of string values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string?[] GetStringValuesOrDefault(string? defaultValue = null)
      {
         return constant.MapArray(x => x.GetStringValueOrDefault(defaultValue));
      }
      
      /// <summary>
      /// Gets the boolean array values of the typed constant, using default formatting.
      /// </summary>
      public bool[] BoolArrayValues => constant.GetBoolValuesOrDefault();

      /// <summary>
      /// Retrieves the boolean array values, or a default value for missing/invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if a boolean element cannot be parsed.</param>
      /// <returns>An array of boolean values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool[] GetBoolValuesOrDefault(bool defaultValue = false)
      {
         return constant.MapArray(x => x.GetBoolValueOrDefault(defaultValue));
      }
      
      /// <summary>
      /// Gets the type symbol array values of the typed constant.
      /// </summary>
      public ITypeSymbol?[] TypeArrayValues => constant.GetTypeValues();

      /// <summary>
      /// Retrieves the type symbol array values.
      /// </summary>
      /// <returns>An array of <see cref="ITypeSymbol"/> values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ITypeSymbol?[] GetTypeValues()
      {
         return constant.MapArray(x => x.GetTypeValue());
      }
      
      /// <summary>
      /// Gets the character array values of the typed constant, using default formatting.
      /// </summary>
      public char[] CharArrayValues => constant.GetCharValuesOrDefault();

      /// <summary>
      /// Retrieves the character array values, or a default value for missing/invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if a character element cannot be parsed.</param>
      /// <returns>An array of character values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public char[] GetCharValuesOrDefault(char defaultValue = '\0')
      {
         return constant.MapArray(x => x.GetCharValueOrDefault(defaultValue));
      }
      
      /// <summary>
      /// Retrieves the generic number array values, or a default value for invalid elements.
      /// </summary>
      /// <typeparam name="T">The numeric type of the array elements.</typeparam>
      /// <param name="defaultValue">The default value to return if a numeric element is invalid.</param>
      /// <returns>An array of generic numeric values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public T[] GetNumberValuesOrDefault<T>(T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return constant.MapArray(x => x.GetNumberValueOrDefault(defaultValue));
      }

      /// <summary>
      /// Gets the integer array values of the typed constant, using default formatting.
      /// </summary>
      public int[] IntArrayValues => constant.GetIntValuesOrDefault();

      /// <summary>
      /// Retrieves the integer array values, or a default value for invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if an integer element is invalid.</param>
      /// <returns>An array of integer values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int[] GetIntValuesOrDefault(int defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }
      
      /// <summary>
      /// Gets the long integer array values of the typed constant, using default formatting.
      /// </summary>
      public long[] LongArrayValues => constant.GetLongValuesOrDefault();

      /// <summary>
      /// Retrieves the long integer array values, or a default value for invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if a long element is invalid.</param>
      /// <returns>An array of long values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public long[] GetLongValuesOrDefault(long defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the float array values of the typed constant, using default formatting.
      /// </summary>
      public float[] FloatArrayValues => constant.GetFloatValuesOrDefault();

      /// <summary>
      /// Retrieves the float array values, or a default value for invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if a float element is invalid.</param>
      /// <returns>An array of float values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public float[] GetFloatValuesOrDefault(float defaultValue = 0.0f)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the double array values of the typed constant, using default formatting.
      /// </summary>
      public double[] DoubleArrayValues => constant.GetDoubleValuesOrDefault();

      /// <summary>
      /// Retrieves the double array values, or a default value for invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if a double element is invalid.</param>
      /// <returns>An array of double values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public double[] GetDoubleValuesOrDefault(double defaultValue = 0.0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the decimal array values of the typed constant, using default formatting.
      /// </summary>
      public decimal[] DecimalArrayValues => constant.GetDecimalValuesOrDefault();
      
      /// <summary>
      /// Retrieves the decimal array values, or a default value for invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if a decimal element is invalid.</param>
      /// <returns>An array of decimal values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public decimal[] GetDecimalValuesOrDefault(decimal defaultValue = 0.0m)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }
      
      /// <summary>
      /// Gets the unsigned integer array values of the typed constant, using default formatting.
      /// </summary>
      public uint[] UIntArrayValues => constant.GetUIntValuesOrDefault();

      /// <summary>
      /// Retrieves the unsigned integer array values, or a default value for invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if an unsigned integer element is invalid.</param>
      /// <returns>An array of unsigned integer values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public uint[] GetUIntValuesOrDefault(uint defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the unsigned long integer array values of the typed constant, using default formatting.
      /// </summary>
      public ulong[] ULongArrayValues => constant.GetULongValuesOrDefault();

      /// <summary>
      /// Retrieves the unsigned long integer array values, or a default value for invalid elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if an unsigned long element is invalid.</param>
      /// <returns>An array of unsigned long values.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ulong[] GetULongValuesOrDefault(ulong defaultValue = 0)
      {
         return constant.GetNumberValuesOrDefault(defaultValue);
      }
      
      /// <summary>
      /// Gets the fully qualified enum name array values of the typed constant, using default formatting.
      /// </summary>
      public string?[] EnumFullNameArrayValues => constant.GetEnumFullNameValuesOrDefault();

      /// <summary>
      /// Retrieves the fully qualified enum name array values, or a default value for null elements.
      /// </summary>
      /// <param name="defaultValue">The default value to return if an enum name is null.</param>
      /// <returns>An array of enum name strings.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string?[] GetEnumFullNameValuesOrDefault(string? defaultValue = null)
      {
         return constant.MapArray(x => x.GetEnumFullNameValueOrDefault(defaultValue));
      }
      
      /// <summary>
      /// Gets the enum field symbol array values of the typed constant.
      /// </summary>
      public IFieldSymbol?[] EnumFieldArrayValues => constant.GetEnumFieldValues();

      /// <summary>
      /// Retrieves the enum field symbol array values.
      /// </summary>
      /// <returns>An array of <see cref="IFieldSymbol"/> values.</returns>
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
