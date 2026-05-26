using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common;

public static class AttributeDataFallbackExtensions
{
   extension(AttributeData attribute)
   {
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? DetermineStringValue(string name, int index, string? defaultValue = null)
      {
         return attribute.GetNamedStringValueOrDefault(name, defaultValue)
            ?? attribute.GetParameterStringValue(index, defaultValue);
      }
      
      public bool DetermineBoolValue(string name, int index, bool defaultValue = false)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolValueOrDefault(defaultValue)
            : attribute.GetParameterBoolValue(index, defaultValue);
      }
      
      public ITypeSymbol? DetermineTypeValue(string name, int index)
      {
         return attribute.GetNamedTypeValueOrDefault(name)
            ?? attribute.GetParameterTypeValue(index);
      }
      
      public char DetermineCharValue(string name, int index, char defaultValue = '\0')
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetCharValueOrDefault(defaultValue)
            : attribute.GetParameterCharValue(index, defaultValue);
      }
      
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? DetermineEnumFullName(string name, int index, string? defaultValue = null)
      {
         return attribute.GetNamedEnumFullNameOrDefault(name, defaultValue)
            ?? attribute.GetParameterEnumFullName(index, defaultValue);
      }

      public IFieldSymbol? DetermineEnumField(string name, int index)
      {
         return attribute.GetNamedEnumMemberOrDefault(name)
            ?? attribute.GetParameterEnumField(index);
      }
      
      public byte DetermineByteValue(string name, int index, byte defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetByteValueOrDefault(defaultValue)
            : attribute.GetParameterByteValue(index, defaultValue);
      }
      
      public short DetermineShortValue(string name, int index, short defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetShortValueOrDefault(defaultValue)
            : attribute.GetParameterShortValue(index, defaultValue);
      }
      
      public int DetermineIntValue(string name, int index, int defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetIntValueOrDefault(defaultValue)
            : attribute.GetParameterIntValue(index, defaultValue);
      }
      
      public long DetermineLongValue(string name, int index, long defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetLongValueOrDefault(defaultValue)
            : attribute.GetParameterLongValue(index, defaultValue);
      }

      public float DetermineFloatValue(string name, int index, float defaultValue = 0.0f)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetFloatValueOrDefault(defaultValue)
            : attribute.GetParameterFloatValue(index, defaultValue);
      }

      public double DetermineDoubleValue(string name, int index, double defaultValue = 0.0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDoubleValueOrDefault(defaultValue)
            : attribute.GetParameterDoubleValue(index, defaultValue);
      }

      public decimal DetermineDecimalValue(string name, int index, decimal defaultValue = 0m)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDecimalValueOrDefault(defaultValue)
            : attribute.GetParameterDecimalValue(index, defaultValue);
      }

      public uint DetermineUIntValue(string name, int index, uint defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetUIntValueOrDefault(defaultValue)
            : attribute.GetParameterUIntValue(index, defaultValue);
      }

      public ulong DetermineULongValue(string name, int index, ulong defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetULongValueOrDefault(defaultValue)
            : attribute.GetParameterULongValue(index, defaultValue);
      }
      
      public string?[] DetermineStringArrayValues(string name, int index, string? defaultValue = null)
      {
         var named = attribute.GetNamedStringArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterStringArrayValues(index, defaultValue);
      }

      public bool[] DetermineBoolArrayValues(string name, int index, bool defaultValue = false)
      {
         var named = attribute.GetNamedBoolArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterBoolArrayValues(index, defaultValue);
      }

      public ITypeSymbol?[] DetermineTypeArrayValues(string name, int index)
      {
         var named = attribute.GetNamedTypeArrayValuesOrDefault(name);
         return named.Length > 0 ? named : attribute.GetParameterTypeArrayValues(index);
      }

      public int[] DetermineIntArrayValues(string name, int index, int defaultValue = 0)
      {
         var named = attribute.GetNamedIntArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterIntArrayValues(index, defaultValue);
      }

      public long[] DetermineLongArrayValues(string name, int index, long defaultValue = 0)
      {
         var named = attribute.GetNamedLongArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterLongArrayValues(index, defaultValue);
      }
      
      public float[] DetermineFloatArrayValues(string name, int index, float defaultValue = 0.0f)
      {
         var named = attribute.GetNamedFloatArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterFloatArrayValues(index, defaultValue);
      }

      public double[] DetermineDoubleArrayValues(string name, int index, double defaultValue = 0.0)
      {
         var named = attribute.GetNamedDoubleArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterDoubleArrayValues(index, defaultValue);
      }
      
      public char[] DetermineCharArrayValues(string name, int index, char defaultValue = '\0')
      {
         var named = attribute.GetNamedCharArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterCharArrayValues(index, defaultValue);
      }
      
      public string?[] DetermineEnumFullNameArrayValues(string name, int index, string? defaultValue = null)
      {
         var named = attribute.GetNamedEnumFullNameArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterEnumFullNameArrayValues(index, defaultValue);
      }

      public IFieldSymbol?[] DetermineEnumFieldArrayValues(string name, int index)
      {
         var named = attribute.GetNamedEnumFieldArrayValuesOrDefault(name);
         return named.Length > 0 ? named : attribute.GetParameterEnumFieldArrayValues(index);
      }
      
      public T DetermineNumberValue<T>(string name, int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberValueOrDefault(defaultValue)
            : attribute.GetParameterNumberValue(index, defaultValue);
      }

      public T[] DetermineNumberArrayValues<T>(string name, int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         var named = attribute.GetNamedNumberArrayValuesOrDefault(name, defaultValue);
         return named.Length > 0 ? named : attribute.GetParameterNumberArrayValues(index, defaultValue);
      }
   }
}

