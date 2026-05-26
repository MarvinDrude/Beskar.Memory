using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.Common;

public static class AttributeDataExtensions
{
   extension(AttributeData attribute)
   {
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
      
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetNamedStringValueOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetStringValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      public bool GetNamedBoolValueOrDefault(string name, bool defaultValue = false)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      public ITypeSymbol? GetNamedTypeValueOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetTypeValue()
            : null;
      }
      
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
      
      [return: NotNullIfNotNull(nameof(defaultValue))]
      public string? GetNamedEnumFullNameOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFullNameValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      public IFieldSymbol? GetNamedEnumMemberOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFieldSymbol()
            : null;
      }
      
      public int GetNamedIntValueOrDefault(string name, int defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetIntValueOrDefault(defaultValue)
            : defaultValue;
      }

      public long GetNamedLongValueOrDefault(string name, long defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetLongValueOrDefault(defaultValue)
            : defaultValue;
      }

      public float GetNamedFloatValueOrDefault(string name, float defaultValue = 0.0f)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetFloatValueOrDefault(defaultValue)
            : defaultValue;
      }

      public double GetNamedDoubleValueOrDefault(string name, double defaultValue = 0.0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDoubleValueOrDefault(defaultValue)
            : defaultValue;
      }

      public byte GetNamedByteValueOrDefault(string name, byte defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetByteValueOrDefault(defaultValue)
            : defaultValue;
      }
      
      public T GetNamedNumberValueOrDefault<T>(string name, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberValueOrDefault(defaultValue)
            : defaultValue;
      }

      public ImmutableArray<TypedConstant> GetNamedArrayValueOrDefault(string name)
      {
         if (attribute.TryGetNamedArgument(name, out var constant) && 
             constant.Kind == TypedConstantKind.Array)
         {
            return constant.Values;
         }
         
         return ImmutableArray<TypedConstant>.Empty;
      }
      
      public string?[] GetNamedStringArrayValuesOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetStringValuesOrDefault(defaultValue)
            : [];
      }
      
      public bool[] GetNamedBoolArrayValuesOrDefault(string name, bool defaultValue = false)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolValuesOrDefault(defaultValue)
            : [];
      }
      
      public ITypeSymbol?[] GetNamedTypeArrayValuesOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetTypeValues()
            : [];
      }
      
      public char[] GetNamedCharArrayValuesOrDefault(string name, char defaultValue = '\0')
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetCharValuesOrDefault(defaultValue)
            : [];
      }

      public int[] GetNamedIntArrayValuesOrDefault(string name, int defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetIntValuesOrDefault(defaultValue)
            : [];
      }

      public long[] GetNamedLongArrayValuesOrDefault(string name, long defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetLongValuesOrDefault(defaultValue)
            : [];
      }

      public float[] GetNamedFloatArrayValuesOrDefault(string name, float defaultValue = 0.0f)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetFloatValuesOrDefault(defaultValue)
            : [];
      }
      
      public decimal[] GetNamedDecimalArrayValuesOrDefault(string name, decimal defaultValue = 0.0m)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDecimalValuesOrDefault(defaultValue)
            : [];
      }

      public double[] GetNamedDoubleArrayValuesOrDefault(string name, double defaultValue = 0.0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetDoubleValuesOrDefault(defaultValue)
            : [];
      }

      public uint[] GetNamedUIntArrayValuesOrDefault(string name, uint defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetUIntValuesOrDefault(defaultValue)
            : [];
      }

      public ulong[] GetNamedULongArrayValuesOrDefault(string name, ulong defaultValue = 0)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetULongValuesOrDefault(defaultValue)
            : [];
      }
      
      public string?[] GetNamedEnumFullNameArrayValuesOrDefault(string name, string? defaultValue = null)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFullNameValuesOrDefault(defaultValue)
            : [];
      }

      public IFieldSymbol?[] GetNamedEnumFieldArrayValuesOrDefault(string name)
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFieldValues()
            : [];
      }

      public T[] GetNamedNumberArrayValuesOrDefault<T>(string name, T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return attribute.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberValuesOrDefault(defaultValue)
            : [];
      }
   }
}

