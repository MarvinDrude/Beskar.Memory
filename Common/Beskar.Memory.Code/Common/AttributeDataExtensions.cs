using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.Common;

/// <summary>
/// Provides extension members for <see cref="AttributeData"/> to retrieve named argument values.
/// </summary>
public static class AttributeDataExtensions
{
   extension(AttributeData attribute)
   {
      /// <summary>
      /// Tries to get the value of a named argument of the attribute.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="value">The retrieved <see cref="TypedConstant"/> value.</param>
      /// <returns><see langword="true"/> if the named argument was found; otherwise, <see langword="false"/>.</returns>
      public bool TryGetNamedArgument(string name, out TypedConstant value)
      {
         foreach (var arg in attribute.NamedArguments.Where(arg => arg.Key == name))
         {
            value = arg.Value;
            return true;
         }
         
         value = default;
         return false;
      }
      
      /// <summary>
      /// Retrieves the string value of a named argument, or a default value if not found or null.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing or null.</param>
      /// <returns>The string value, or the default value.</returns>
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetNamedStringValueOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetStringValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      /// <summary>
      /// Retrieves the boolean value of a named argument, or a default value if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing.</param>
      /// <returns>The boolean value, or the default value.</returns>
      public bool GetNamedBoolValueOrDefault(string name, bool defaultValue = false)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      /// <summary>
      /// Retrieves the type symbol of a named argument, or <see langword="null"/> if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <returns>The <see cref="ITypeSymbol"/> value, or <see langword="null"/>.</returns>
      public ITypeSymbol? GetNamedTypeValueOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetTypeValue()
            : null;
      }
      
      /// <summary>
      /// Retrieves the C# string representation of a named argument, or <see langword="null"/> if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <returns>The C# code representation of the argument, or <see langword="null"/>.</returns>
      public string? GetCSharpString(string name)
      {
         if (!attribute.TryGetNamedArgument(name, out var constant))
         {
            return null;
         }

         return constant.IsArray 
            ? $"[{string.Join(", ", constant.Values.Select(v => v.ToCSharpString()))}]" 
            : constant.ToCSharpString();
      }
      
      /// <summary>
      /// Retrieves the fully qualified enum name of a named argument, or a default value if not found or null.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing or null.</param>
      /// <returns>The fully qualified enum name, or the default value.</returns>
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetNamedEnumFullNameOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFullNameValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      /// <summary>
      /// Retrieves the enum field symbol of a named argument, or <see langword="null"/> if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <returns>The <see cref="IFieldSymbol"/> representing the enum field, or <see langword="null"/>.</returns>
      public IFieldSymbol? GetNamedEnumMemberOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFieldSymbol()
            : null;
      }
      
      /// <summary>
      /// Retrieves the integer value of a named argument, or a default value if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing.</param>
      /// <returns>The integer value, or the default value.</returns>
      public int GetNamedIntValueOrDefault(string name, int defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetIntValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the long integer value of a named argument, or a default value if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing.</param>
      /// <returns>The long integer value, or the default value.</returns>
      public long GetNamedLongValueOrDefault(string name, long defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetLongValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the float value of a named argument, or a default value if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing.</param>
      /// <returns>The float value, or the default value.</returns>
      public float GetNamedFloatValueOrDefault(string name, float defaultValue = 0.0f)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetFloatValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the double value of a named argument, or a default value if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing.</param>
      /// <returns>The double value, or the default value.</returns>
      public double GetNamedDoubleValueOrDefault(string name, double defaultValue = 0.0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDoubleValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the byte value of a named argument, or a default value if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing.</param>
      /// <returns>The byte value, or the default value.</returns>
      public byte GetNamedByteValueOrDefault(string name, byte defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetByteValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      /// <summary>
      /// Retrieves the generic number value of a named argument, or a default value if not found.
      /// </summary>
      /// <typeparam name="T">The numeric type of the argument.</typeparam>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return if the argument is missing.</param>
      /// <returns>The numeric value of type <typeparamref name="T"/>, or the default value.</returns>
      public T GetNamedNumberValueOrDefault<T>(string name, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberValueOrDefault(defaultValue)
            : defaultValue;
      }

      /// <summary>
      /// Retrieves the array of typed constants for a named argument, or an empty array if not found or not an array.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <returns>An array of <see cref="TypedConstant"/> values.</returns>
      public ImmutableArray<TypedConstant> GetNamedArrayValueOrDefault(string name)
      {
         if (attribute.TryGetNamedArgument(name, out var constant) && 
             constant.Kind == TypedConstantKind.Array)
         {
            return constant.Values;
         }
         
         return ImmutableArray<TypedConstant>.Empty;
      }
      
      /// <summary>
      /// Retrieves the string array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for null elements.</param>
      /// <returns>An array of string values.</returns>
      public string?[] GetNamedStringArrayValuesOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetStringValuesOrDefault(defaultValue)
            : [];
      }
      
      /// <summary>
      /// Retrieves the boolean array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of boolean values.</returns>
      public bool[] GetNamedBoolArrayValuesOrDefault(string name, bool defaultValue = false)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolValuesOrDefault(defaultValue)
            : [];
      }
      
      /// <summary>
      /// Retrieves the type symbol array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <returns>An array of <see cref="ITypeSymbol"/> values.</returns>
      public ITypeSymbol?[] GetNamedTypeArrayValuesOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetTypeValues()
            : [];
      }
      
      /// <summary>
      /// Retrieves the character array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of character values.</returns>
      public char[] GetNamedCharArrayValuesOrDefault(string name, char defaultValue = '\0')
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetCharValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the integer array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of integer values.</returns>
      public int[] GetNamedIntArrayValuesOrDefault(string name, int defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetIntValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the long integer array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of long integer values.</returns>
      public long[] GetNamedLongArrayValuesOrDefault(string name, long defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetLongValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the float array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of float values.</returns>
      public float[] GetNamedFloatArrayValuesOrDefault(string name, float defaultValue = 0.0f)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetFloatValuesOrDefault(defaultValue)
            : [];
      }
      
      /// <summary>
      /// Retrieves the decimal array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of decimal values.</returns>
      public decimal[] GetNamedDecimalArrayValuesOrDefault(string name, decimal defaultValue = 0.0m)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDecimalValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the double array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of double values.</returns>
      public double[] GetNamedDoubleArrayValuesOrDefault(string name, double defaultValue = 0.0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDoubleValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the unsigned integer array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of unsigned integer values.</returns>
      public uint[] GetNamedUIntArrayValuesOrDefault(string name, uint defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetUIntValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the unsigned long integer array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of unsigned long integer values.</returns>
      public ulong[] GetNamedULongArrayValuesOrDefault(string name, ulong defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetULongValuesOrDefault(defaultValue)
            : [];
      }
      
      /// <summary>
      /// Retrieves the fully qualified enum name array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for null/invalid elements.</param>
      /// <returns>An array of fully qualified enum names.</returns>
      public string?[] GetNamedEnumFullNameArrayValuesOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFullNameValuesOrDefault(defaultValue)
            : [];
      }

      /// <summary>
      /// Retrieves the enum field symbol array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <param name="name">The name of the argument.</param>
      /// <returns>An array of <see cref="IFieldSymbol"/> values.</returns>
      public IFieldSymbol?[] GetNamedEnumFieldArrayValuesOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFieldValues()
            : [];
      }

      /// <summary>
      /// Retrieves the generic number array values of a named argument, or an empty array if not found.
      /// </summary>
      /// <typeparam name="T">The numeric type of the array elements.</typeparam>
      /// <param name="name">The name of the argument.</param>
      /// <param name="defaultValue">The default value to return for invalid elements.</param>
      /// <returns>An array of numeric values of type <typeparamref name="T"/>.</returns>
      public T[] GetNamedNumberArrayValuesOrDefault<T>(string name, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberValuesOrDefault(defaultValue)
            : [];
      }
   }
}
