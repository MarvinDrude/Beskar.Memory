using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Beskar.Memory.Code.Common;

/// <summary>
/// Provides extension members for <see cref="TypedConstant"/> to retrieve metadata and primitive/enum values.
/// </summary>
public static class TypedConstantExtensions
{
   extension(TypedConstant constant)
   {
      /// <summary>
      /// Gets a value indicating whether the typed constant represents an error.
      /// </summary>
      public bool IsError => constant.Kind == TypedConstantKind.Error;

      /// <summary>
      /// Gets a value indicating whether the typed constant represents a primitive type.
      /// </summary>
      public bool IsPrimitive => constant.Kind == TypedConstantKind.Primitive;

      /// <summary>
      /// Gets a value indicating whether the typed constant represents an enum.
      /// </summary>
      public bool IsEnum => constant.Kind == TypedConstantKind.Enum;

      /// <summary>
      /// Gets a value indicating whether the typed constant represents a type.
      /// </summary>
      public bool IsType => constant.Kind == TypedConstantKind.Type;
      
      /// <summary>
      /// Gets the string value of the typed constant using the default configuration.
      /// </summary>
      public string? StringValue => constant.GetStringValueOrDefault();

      /// <summary>
      /// Retrieves the string value, or a default value if the constant is null.
      /// </summary>
      /// <param name="defaultValue">The default value to return if the constant is null.</param>
      /// <returns>The string value, or the default value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string? GetStringValueOrDefault(string? defaultValue = null)
      {
         if (constant.IsNull) return defaultValue;
         return constant.Value as string ?? defaultValue;
      }

      /// <summary>
      /// Gets the boolean value of the typed constant using the default configuration.
      /// </summary>
      public bool BoolValue => constant.GetBoolValueOrDefault();
      
      /// <summary>
      /// Retrieves the boolean value, or a default value if the value is not boolean.
      /// </summary>
      /// <param name="defaultValue">The default value to return if the constant value is not a boolean.</param>
      /// <returns>The boolean value, or the default value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool GetBoolValueOrDefault(bool defaultValue = false)
      {
         return constant.Value is bool boolValue 
            ? boolValue : defaultValue;
      }

      /// <summary>
      /// Gets the type symbol representation of the typed constant.
      /// </summary>
      public ITypeSymbol? TypeValue => constant.GetTypeValue();
      
      /// <summary>
      /// Retrieves the type symbol representation.
      /// </summary>
      /// <returns>The <see cref="ITypeSymbol"/> value, or <see langword="null"/> if the constant is not a type.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ITypeSymbol? GetTypeValue()
      {
         return constant.IsType 
            ? constant.Value as ITypeSymbol
            : null;
      }

      /// <summary>
      /// Gets the fully qualified name of the enum constant using the default configuration.
      /// </summary>
      public string? EnumFullNameValue => constant.GetEnumFullNameValueOrDefault();
      
      /// <summary>
      /// Retrieves the fully qualified name of the enum constant, or a default value if the constant is not an enum.
      /// </summary>
      /// <param name="defaultValue">The default value to return if the constant is not an enum.</param>
      /// <returns>The fully qualified enum name string, or the default value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public string? GetEnumFullNameValueOrDefault(string? defaultValue = null)
      {
         return constant is { Kind: TypedConstantKind.Enum }
            ? constant.ToCSharpString() : defaultValue;
      }

      /// <summary>
      /// Gets the enum field symbol of the typed constant.
      /// </summary>
      public IFieldSymbol? EnumMember => constant.GetEnumFieldSymbol();
      
      /// <summary>
      /// Retrieves the enum field symbol representing the enum constant value.
      /// </summary>
      /// <returns>The <see cref="IFieldSymbol"/> representing the enum field, or <see langword="null"/> if not an enum.</returns>
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
      
      /// <summary>
      /// Gets the character value of the typed constant using the default configuration.
      /// </summary>
      public char CharValue => constant.GetCharValueOrDefault();

      /// <summary>
      /// Retrieves the character value, or a default value if the value is not a character.
      /// </summary>
      /// <param name="defaultValue">The default value to return if the constant is not a character.</param>
      /// <returns>The character value, or the default value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public char GetCharValueOrDefault(char defaultValue = '\0')
      {
         return constant.Value is char charValue ? charValue : defaultValue;
      }
      
      /// <summary>
      /// Retrieves the generic number value, or a default value if the constant is not that numeric type.
      /// </summary>
      /// <typeparam name="T">The numeric type to retrieve.</typeparam>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The numeric value of type <typeparamref name="T"/>.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public T GetNumberValueOrDefault<T>(T defaultValue = default) 
         where T : struct, INumber<T>
      {
         return constant.Value is T  numberValue
            ? numberValue : defaultValue;
      }

      /// <summary>
      /// Gets the integer value of the typed constant using the default configuration.
      /// </summary>
      public int IntValue => constant.GetIntValueOrDefault();

      /// <summary>
      /// Retrieves the integer value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The integer value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int GetIntValueOrDefault(int defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the long integer value of the typed constant using the default configuration.
      /// </summary>
      public long LongValue => constant.GetLongValueOrDefault();
      
      /// <summary>
      /// Retrieves the long integer value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The long integer value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public long GetLongValueOrDefault(long defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }
      
      /// <summary>
      /// Gets the float value of the typed constant using the default configuration.
      /// </summary>
      public float FloatValue => constant.GetFloatValueOrDefault();

      /// <summary>
      /// Retrieves the float value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The float value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public float GetFloatValueOrDefault(float defaultValue = 0.0f)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the double value of the typed constant using the default configuration.
      /// </summary>
      public double DoubleValue => constant.GetDoubleValueOrDefault();

      /// <summary>
      /// Retrieves the double value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The double value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public double GetDoubleValueOrDefault(double defaultValue = 0.0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the decimal value of the typed constant using the default configuration.
      /// </summary>
      public decimal DecimalValue => constant.GetDecimalValueOrDefault();

      /// <summary>
      /// Retrieves the decimal value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The decimal value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public decimal GetDecimalValueOrDefault(decimal defaultValue = 0m)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the byte value of the typed constant using the default configuration.
      /// </summary>
      public byte ByteValue => constant.GetByteValueOrDefault();

      /// <summary>
      /// Retrieves the byte value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The byte value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public byte GetByteValueOrDefault(byte defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the short integer value of the typed constant using the default configuration.
      /// </summary>
      public short ShortValue => constant.GetShortValueOrDefault();

      /// <summary>
      /// Retrieves the short integer value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The short integer value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public short GetShortValueOrDefault(short defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the unsigned integer value of the typed constant using the default configuration.
      /// </summary>
      public uint UIntValue => constant.GetUIntValueOrDefault();

      /// <summary>
      /// Retrieves the unsigned integer value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The unsigned integer value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public uint GetUIntValueOrDefault(uint defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }

      /// <summary>
      /// Gets the unsigned long integer value of the typed constant using the default configuration.
      /// </summary>
      public ulong ULongValue => constant.GetULongValueOrDefault();

      /// <summary>
      /// Retrieves the unsigned long integer value, or a default value.
      /// </summary>
      /// <param name="defaultValue">The default value to return.</param>
      /// <returns>The unsigned long integer value.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ulong GetULongValueOrDefault(ulong defaultValue = 0)
      {
         return constant.GetNumberValueOrDefault(defaultValue);
      }
   }
}
