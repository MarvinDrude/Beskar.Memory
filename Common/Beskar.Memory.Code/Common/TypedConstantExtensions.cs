using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.Common;

public static class TypedConstantExtensions
{
   extension(TypedConstant constant)
   {
      public bool IsError => constant.Kind == TypedConstantKind.Error;
      public bool IsPrimitive => constant.Kind == TypedConstantKind.Primitive;
      public bool IsEnum => constant.Kind == TypedConstantKind.Enum;
      public bool IsType => constant.Kind == TypedConstantKind.Type;
      
      public string? StringValue => constant.GetStringValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string? GetStringValueOrDefault(string? defaultValue = null)
      {
         if (constant.IsNull) return defaultValue;
         return constant.Value as string ?? defaultValue;
      }

      public bool BoolValue => constant.GetBoolValueOrDefault();
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool GetBoolValueOrDefault(bool defaultValue = false)
      {
         return constant.Value is bool boolValue 
            ? boolValue : defaultValue;
      }

      public ITypeSymbol? TypeValue => constant.GetTypeValue();
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ITypeSymbol? GetTypeValue()
      {
         return constant.IsType 
            ? constant.Value as ITypeSymbol
            : null;
      }

      public string? EnumFullNameValue => constant.GetEnumFullNameValueOrDefault();
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string? GetEnumFullNameValueOrDefault(string? defaultValue = null)
      {
         return constant is { Kind: TypedConstantKind.Enum }
            ? constant.ToCSharpString() : defaultValue;
      }

      public IFieldSymbol? EnumMember => constant.GetEnumFieldSymbol();
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public IFieldSymbol? GetEnumFieldSymbol()
      {
         if (constant is not { Kind: TypedConstantKind.Enum, 
                Type: INamedTypeSymbol enumType, Value: not null })
         {
            return null;
         }

         return enumType.GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(f => f.HasConstantValue && Equals(f.ConstantValue, constant.Value));
      }
      
      public char CharValue => constant.GetCharValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public char GetCharValueOrDefault(char defaultValue = '\0')
      {
         return constant.Value is char charValue ? charValue : defaultValue;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public T GetNumberValueOrDefault<T>(T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return constant.Value is T  numberValue
            ? numberValue : defaultValue;
      }

      public int IntValue => constant.GetIntValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int GetIntValueOrDefault(int defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      public long LongValue => constant.GetLongValueOrDefault();
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public long GetLongValueOrDefault(long defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }
      
      public float FloatValue => constant.GetFloatValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public float GetFloatValueOrDefault(float defaultValue = 0.0f)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      public double DoubleValue => constant.GetDoubleValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public double GetDoubleValueOrDefault(double defaultValue = 0.0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      public decimal DecimalValue => constant.GetDecimalValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public decimal GetDecimalValueOrDefault(decimal defaultValue = 0m)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      public byte ByteValue => constant.GetByteValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public byte GetByteValueOrDefault(byte defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      public short ShortValue => constant.GetShortValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public short GetShortValueOrDefault(short defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      public uint UIntValue => constant.GetUIntValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public uint GetUIntValueOrDefault(uint defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      public ulong ULongValue => constant.GetULongValueOrDefault();

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ulong GetULongValueOrDefault(ulong defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }
   }
}

