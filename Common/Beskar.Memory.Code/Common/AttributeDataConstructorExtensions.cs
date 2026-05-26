using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.Common;

public static class AttributeDataConstructorExtensions
{
   extension(AttributeData attribute)
   {
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetParameterStringValue(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetStringValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      public bool GetParameterBoolValue(int index, bool defaultValue = false)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetBoolValueOrDefault(defaultValue)
            : defaultValue;
      }

      public ITypeSymbol? GetParameterTypeValue(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetTypeValue()
            : null;
      }

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

      public byte GetParameterByteValue(int index, byte defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetByteValueOrDefault(defaultValue)
            : defaultValue;
      }

      public short GetParameterShortValue(int index, short defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetShortValueOrDefault(defaultValue)
            : defaultValue;
      }

      public int GetParameterIntValue(int index, int defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetIntValueOrDefault(defaultValue)
            : defaultValue;
      }

      public long GetParameterLongValue(int index, long defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetLongValueOrDefault(defaultValue)
            : defaultValue;
      }

      public float GetParameterFloatValue(int index, float defaultValue = 0.0f)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetFloatValueOrDefault(defaultValue)
            : defaultValue;
      }

      public double GetParameterDoubleValue(int index, double defaultValue = 0.0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetDoubleValueOrDefault(defaultValue)
            : defaultValue;
      }

      public decimal GetParameterDecimalValue(int index, decimal defaultValue = 0m)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetDecimalValueOrDefault(defaultValue)
            : defaultValue;
      }

      public uint GetParameterUIntValue(int index, uint defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetUIntValueOrDefault(defaultValue)
            : defaultValue;
      }

      public ulong GetParameterULongValue(int index, ulong defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetULongValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      public char GetParameterCharValue(int index, char defaultValue = '\0')
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetCharValueOrDefault(defaultValue)
            : defaultValue;
      }

      public string?[] GetParameterStringArrayValues(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetStringValuesOrDefault(defaultValue)
            : [];
      }

      public bool[] GetParameterBoolArrayValues(int index, bool defaultValue = false)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetBoolValuesOrDefault(defaultValue)
            : [];
      }

      public ITypeSymbol?[] GetParameterTypeArrayValues(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetTypeValues()
            : [];
      }

      public int[] GetParameterIntArrayValues(int index, int defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetIntValuesOrDefault(defaultValue)
            : [];
      }

      public long[] GetParameterLongArrayValues(int index, long defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetLongValuesOrDefault(defaultValue)
            : [];
      }
      
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetParameterEnumFullName(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFullNameValueOrDefault(defaultValue)
            : defaultValue;
      }

      public IFieldSymbol? GetParameterEnumField(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFieldSymbol()
            : null;
      }

      public float[] GetParameterFloatArrayValues(int index, float defaultValue = 0.0f)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetFloatValuesOrDefault(defaultValue)
            : [];
      }

      public double[] GetParameterDoubleArrayValues(int index, double defaultValue = 0.0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetDoubleValuesOrDefault(defaultValue)
            : [];
      }
      
      public char[] GetParameterCharArrayValues(int index, char defaultValue = '\0')
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetCharValuesOrDefault(defaultValue)
            : [];
      }

      public uint[] GetParameterUIntArrayValues(int index, uint defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetUIntValuesOrDefault(defaultValue)
            : [];
      }

      public ulong[] GetParameterULongArrayValues(int index, ulong defaultValue = 0)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetULongValuesOrDefault(defaultValue)
            : [];
      }

      public string?[] GetParameterEnumFullNameArrayValues(int index, string? defaultValue = null)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFullNameValuesOrDefault(defaultValue)
            : [];
      }

      public IFieldSymbol?[] GetParameterEnumFieldArrayValues(int index)
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetEnumFieldValues()
            : [];
      }

      public T GetParameterNumberValue<T>(int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetNumberValueOrDefault(defaultValue)
            : defaultValue;
      }

      public T[] GetParameterNumberArrayValues<T>(int index, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.ConstructorArguments.Length > index
            ? attribute.ConstructorArguments[index].GetNumberValuesOrDefault(defaultValue)
            : [];
      }
   }
}

