using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common;

/// <summary>
/// Provides extension members for <see cref="AttributeData"/> to determine argument values by checking named arguments first, falling back to positional constructor parameters.
/// </summary>
public static class AttributeDataFallbackExtensions
{
   extension(AttributeData attribute)
   {
      /// <summary>
      /// Determines the string value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found or is null.</param>
      /// <returns>The determined string value, or the default value.</returns>
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? DetermineStringValue(string name, int index, string? defaultValue = null)
      {
         return attribute.GetNamedStringValueOrDefault(name, defaultValue)
            ?? attribute.GetParameterStringValue(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the boolean value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined boolean value.</returns>
      public bool DetermineBoolValue(string name, int index, bool defaultValue = false)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolValueOrDefault(defaultValue)
            : attribute.GetParameterBoolValue(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the type symbol value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <returns>The determined <see cref="ITypeSymbol"/> value, or <see langword="null"/>.</returns>
      public ITypeSymbol? DetermineTypeValue(string name, int index)
      {
         return attribute.GetNamedTypeValueOrDefault(name)
            ?? attribute.GetParameterTypeValue(index);
      }
      
      /// <summary>
      /// Determines the character value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined character value.</returns>
      public char DetermineCharValue(string name, int index, char defaultValue = '\0')
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetCharValueOrDefault(defaultValue)
            : attribute.GetParameterCharValue(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the fully qualified enum name by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found or is null.</param>
      /// <returns>The determined enum full name, or the default value.</returns>
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? DetermineEnumFullName(string name, int index, string? defaultValue = null)
      {
         return attribute.GetNamedEnumFullNameOrDefault(name, defaultValue)
            ?? attribute.GetParameterEnumFullName(index, defaultValue);
      }

      /// <summary>
      /// Determines the enum field symbol by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <returns>The determined <see cref="IFieldSymbol"/>, or <see langword="null"/>.</returns>
      public IFieldSymbol? DetermineEnumField(string name, int index)
      {
         return attribute.GetNamedEnumMemberOrDefault(name)
            ?? attribute.GetParameterEnumField(index);
      }
      
      /// <summary>
      /// Determines the byte value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined byte value.</returns>
      public byte DetermineByteValue(string name, int index, byte defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetByteValueOrDefault(defaultValue)
            : attribute.GetParameterByteValue(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the short value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined short value.</returns>
      public short DetermineShortValue(string name, int index, short defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetShortValueOrDefault(defaultValue)
            : attribute.GetParameterShortValue(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the integer value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined integer value.</returns>
      public int DetermineIntValue(string name, int index, int defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetIntValueOrDefault(defaultValue)
            : attribute.GetParameterIntValue(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the long value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined long value.</returns>
      public long DetermineLongValue(string name, int index, long defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetLongValueOrDefault(defaultValue)
            : attribute.GetParameterLongValue(index, defaultValue);
      }

      /// <summary>
      /// Determines the float value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined float value.</returns>
      public float DetermineFloatValue(string name, int index, float defaultValue = 0.0f)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetFloatValueOrDefault(defaultValue)
            : attribute.GetParameterFloatValue(index, defaultValue);
      }

      /// <summary>
      /// Determines the double value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined double value.</returns>
      public double DetermineDoubleValue(string name, int index, double defaultValue = 0.0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDoubleValueOrDefault(defaultValue)
            : attribute.GetParameterDoubleValue(index, defaultValue);
      }

      /// <summary>
      /// Determines the decimal value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined decimal value.</returns>
      public decimal DetermineDecimalValue(string name, int index, decimal defaultValue = 0m)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDecimalValueOrDefault(defaultValue)
            : attribute.GetParameterDecimalValue(index, defaultValue);
      }

      /// <summary>
      /// Determines the unsigned integer value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined unsigned integer value.</returns>
      public uint DetermineUIntValue(string name, int index, uint defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetUIntValueOrDefault(defaultValue)
            : attribute.GetParameterUIntValue(index, defaultValue);
      }

      /// <summary>
      /// Determines the unsigned long value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The determined unsigned long value.</returns>
      public ulong DetermineULongValue(string name, int index, ulong defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetULongValueOrDefault(defaultValue)
            : attribute.GetParameterULongValue(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the string array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for null elements.</param>
      /// <returns>An array of string values.</returns>
      public string?[] DetermineStringArrayValues(string name, int index, string? defaultValue = null)
      {
         var named = attribute.GetNamedStringArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterStringArrayValues(index, defaultValue);
      }

      /// <summary>
      /// Determines the boolean array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of boolean values.</returns>
      public bool[] DetermineBoolArrayValues(string name, int index, bool defaultValue = false)
      {
         var named = attribute.GetNamedBoolArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterBoolArrayValues(index, defaultValue);
      }

      /// <summary>
      /// Determines the type symbol array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <returns>An array of <see cref="ITypeSymbol"/> values.</returns>
      public ITypeSymbol?[] DetermineTypeArrayValues(string name, int index)
      {
         var named = attribute.GetNamedTypeArrayValuesOrDefault(name);
         return named.Length > 0 ? named : attribute.GetParameterTypeArrayValues(index);
      }

      /// <summary>
      /// Determines the integer array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of integer values.</returns>
      public int[] DetermineIntArrayValues(string name, int index, int defaultValue = 0)
      {
         var named = attribute.GetNamedIntArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterIntArrayValues(index, defaultValue);
      }

      /// <summary>
      /// Determines the long array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of long values.</returns>
      public long[] DetermineLongArrayValues(string name, int index, long defaultValue = 0)
      {
         var named = attribute.GetNamedLongArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterLongArrayValues(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the float array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of float values.</returns>
      public float[] DetermineFloatArrayValues(string name, int index, float defaultValue = 0.0f)
      {
         var named = attribute.GetNamedFloatArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterFloatArrayValues(index, defaultValue);
      }

      /// <summary>
      /// Determines the double array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of double values.</returns>
      public double[] DetermineDoubleArrayValues(string name, int index, double defaultValue = 0.0)
      {
         var named = attribute.GetNamedDoubleArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterDoubleArrayValues(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the character array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of character values.</returns>
      public char[] DetermineCharArrayValues(string name, int index, char defaultValue = '\0')
      {
         var named = attribute.GetNamedCharArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterCharArrayValues(index, defaultValue);
      }
      
      /// <summary>
      /// Determines the fully qualified enum name array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for null/invalid elements.</param>
      /// <returns>An array of enum full name strings.</returns>
      public string?[] DetermineEnumFullNameArrayValues(string name, int index, string? defaultValue = null)
      {
         var named = attribute.GetNamedEnumFullNameArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterEnumFullNameArrayValues(index, defaultValue);
      }

      /// <summary>
      /// Determines the enum field symbol array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <returns>An array of <see cref="IFieldSymbol"/> values.</returns>
      public IFieldSymbol?[] DetermineEnumFieldArrayValues(string name, int index)
      {
         var named = attribute.GetNamedEnumFieldArrayValuesOrDefault(name);
         return named.Length > 0 ? named : attribute.GetParameterEnumFieldArrayValues(index);
      }
      
      /// <summary>
      /// Determines the generic number value by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <typeparam name="T">The numeric type of the parameter.</typeparam>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return if the argument is not found.</param>
      /// <returns>The numeric value of type <typeparamref name="T"/>.</returns>
      public T DetermineNumberValue<T>(string name, int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberValueOrDefault(defaultValue)
            : attribute.GetParameterNumberValue(index, defaultValue);
      }

      /// <summary>
      /// Determines the generic number array values by checking a named argument first, falling back to a positional parameter at the specified index.
      /// </summary>
      /// <typeparam name="T">The numeric type of the array elements.</typeparam>
      /// <param name="name">The name of the argument.</param>
      /// <param name="index">The positional constructor parameter index.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of numeric values of type <typeparamref name="T"/>.</returns>
      public T[] DetermineNumberArrayValues<T>(string name, int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         var named = attribute.GetNamedNumberArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterNumberArrayValues(index, defaultValue);
      }
   }
}
