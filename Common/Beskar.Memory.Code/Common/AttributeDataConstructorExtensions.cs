using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.Common;

/// <summary>
/// Provides extension members for <see cref="AttributeData"/> to retrieve constructor parameter values.
/// </summary>
public static class AttributeDataConstructorExtensions
{
   extension(AttributeData attribute)
   {
      /// <summary>
      /// Retrieves the string value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds or the value is null.</param>
      /// <returns>The string value, or the default value if not found.</returns>
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetParameterStringValue(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetStringValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      /// <summary>
      /// Retrieves the boolean value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The boolean value, or the default value if not found.</returns>
      public bool GetParameterBoolValue(int index, bool defaultValue = false)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetBoolValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the type symbol value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <returns>The <see cref="ITypeSymbol"/> value, or <see langword="null"/> if not found.</returns>
      public ITypeSymbol? GetParameterTypeValue(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetTypeValue()
            : null;
      }

      /// <summary>
      /// Retrieves the C# string representation of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <returns>A string containing the C# code representation of the parameter, or <see langword="null"/> if not found.</returns>
      public string? GetCSharpString(int index)
      {
         if (attribute.ConstructorArguments.Length <= index)
         {
            return null;
         }
         
         var arg = attribute.ConstructorArguments[index];
         if (arg.IsArray)
         {
            return $"[{string.Join(", ", arg.Values.Select(v => v.ToCSharpString()))}]";
         }
         else
         {
            return arg.ToCSharpString();
         }
      }

      /// <summary>
      /// Retrieves the byte value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The byte value, or the default value if not found.</returns>
      public byte GetParameterByteValue(int index, byte defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetByteValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the short integer value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The short integer value, or the default value if not found.</returns>
      public short GetParameterShortValue(int index, short defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetShortValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the integer value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The integer value, or the default value if not found.</returns>
      public int GetParameterIntValue(int index, int defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetIntValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the long integer value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The long integer value, or the default value if not found.</returns>
      public long GetParameterLongValue(int index, long defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetLongValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the single-precision floating-point value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The float value, or the default value if not found.</returns>
      public float GetParameterFloatValue(int index, float defaultValue = 0.0f)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetFloatValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the double-precision floating-point value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The double value, or the default value if not found.</returns>
      public double GetParameterDoubleValue(int index, double defaultValue = 0.0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetDoubleValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the decimal value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The decimal value, or the default value if not found.</returns>
      public decimal GetParameterDecimalValue(int index, decimal defaultValue = 0m)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetDecimalValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the unsigned integer value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The unsigned integer value, or the default value if not found.</returns>
      public uint GetParameterUIntValue(int index, uint defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetUIntValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the unsigned long integer value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The unsigned long integer value, or the default value if not found.</returns>
      public ulong GetParameterULongValue(int index, ulong defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetULongValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      /// <summary>
      /// Retrieves the character value of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The character value, or the default value if not found.</returns>
      public char GetParameterCharValue(int index, char defaultValue = '\0')
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetCharValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the string array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is null.</param>
      /// <returns>An array of string values, or an empty array if not found.</returns>
      public string?[] GetParameterStringArrayValues(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetStringValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the boolean array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is missing or invalid.</param>
      /// <returns>An array of boolean values, or an empty array if not found.</returns>
      public bool[] GetParameterBoolArrayValues(int index, bool defaultValue = false)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetBoolValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the type symbol array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <returns>An array of <see cref="ITypeSymbol"/> values, or an empty array if not found.</returns>
      public ITypeSymbol?[] GetParameterTypeArrayValues(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetTypeValues()
            : [];
      }

      /// <summary>
      /// Retrieves the integer array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of integer values, or an empty array if not found.</returns>
      public int[] GetParameterIntArrayValues(int index, int defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetIntValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the long integer array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of long integer values, or an empty array if not found.</returns>
      public long[] GetParameterLongArrayValues(int index, long defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetLongValuesOrDefault(defaultValue)
            : [];
      }
      
      /// <summary>
      /// Retrieves the fully qualified enum name of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds or null.</param>
      /// <returns>The fully qualified enum name, or the default value if not found.</returns>
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetParameterEnumFullName(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFullNameValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the enum field symbol of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <returns>The <see cref="IFieldSymbol"/> representing the enum field, or <see langword="null"/> if not found.</returns>
      public IFieldSymbol? GetParameterEnumField(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFieldSymbol()
            : null;
      }

      /// <summary>
      /// Retrieves the float array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of float values, or an empty array if not found.</returns>
      public float[] GetParameterFloatArrayValues(int index, float defaultValue = 0.0f)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetFloatValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the double array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of double values, or an empty array if not found.</returns>
      public double[] GetParameterDoubleArrayValues(int index, double defaultValue = 0.0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetDoubleValuesOrDefault(defaultValue)
            : [];
      }
      
      /// <summary>
      /// Retrieves the character array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of character values, or an empty array if not found.</returns>
      public char[] GetParameterCharArrayValues(int index, char defaultValue = '\0')
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetCharValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the unsigned integer array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of unsigned integer values, or an empty array if not found.</returns>
      public uint[] GetParameterUIntArrayValues(int index, uint defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetUIntValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the unsigned long integer array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of unsigned long integer values, or an empty array if not found.</returns>
      public ulong[] GetParameterULongArrayValues(int index, ulong defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetULongValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the fully qualified enum name array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is null or invalid.</param>
      /// <returns>An array of fully qualified enum names, or an empty array if not found.</returns>
      public string?[] GetParameterEnumFullNameArrayValues(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFullNameValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the enum field symbol array values of the constructor parameter at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <returns>An array of <see cref="IFieldSymbol"/> values, or an empty array if not found.</returns>
      public IFieldSymbol?[] GetParameterEnumFieldArrayValues(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFieldValues()
            : [];
      }

      /// <summary>
      /// Retrieves the generic number value of the constructor parameter at the specified index.
      /// </summary>
      /// <typeparam name="T">The numeric type of the parameter.</typeparam>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if the index is out of bounds.</param>
      /// <returns>The numeric value of type <typeparamref name="T"/>, or the default value if not found.</returns>
      public T GetParameterNumberValue<T>(int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetNumberValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the generic number array values of the constructor parameter at the specified index.
      /// </summary>
      /// <typeparam name="T">The numeric type of the array elements.</typeparam>
      /// <param name="index">The zero-based index of the constructor parameter.</param>
      /// <param name="defaultValue">The default value to return if an array element is invalid.</param>
      /// <returns>An array of numeric values of type <typeparamref name="T"/>, or an empty array if not found.</returns>
      public T[] GetParameterNumberArrayValues<T>(int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetNumberValuesOrDefault(defaultValue)
            : [];
      }
   }
}
