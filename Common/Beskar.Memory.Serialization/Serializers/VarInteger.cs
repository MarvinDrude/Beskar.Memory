using System.Buffers;
using System.Runtime.CompilerServices;
using Beskar.Memory.Writers;

namespace Beskar.Memory.Serialization.Serializers;

/// <summary>
/// High-performance Varint (LEB128) encoding helper with ZigZag mapping for signed integers.
/// </summary>
public static class VarInteger
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write(ref BufferWriter<byte> writer, uint value)
    {
        var bytesWritten = 0;
        while (value >= 0x80)
        {
            writer.Add((byte)(value | 0x80));
            value >>= 7;
            bytesWritten++;
        }
        writer.Add((byte)value);
        bytesWritten++;
        return bytesWritten;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write(ref BufferWriter<byte> writer, int value)
    {
        var zigzag = (uint)((value << 1) ^ (value >> 31));
        return Write(ref writer, zigzag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write(ref BufferWriter<byte> writer, ulong value)
    {
        var bytesWritten = 0;
        while (value >= 0x80)
        {
            writer.Add((byte)(value | 0x80));
            value >>= 7;
            bytesWritten++;
        }
        writer.Add((byte)value);
        bytesWritten++;
        return bytesWritten;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write(ref BufferWriter<byte> writer, long value)
    {
        var zigzag = (ulong)((value << 1) ^ (value >> 63));
        return Write(ref writer, zigzag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryRead(ref SequenceReader<byte> reader, out uint value)
    {
        value = 0;
        var shift = 0;
        while (shift < 35) // 5 bytes max for 32-bit uint
        {
            if (!reader.TryRead(out byte b))
            {
                return false;
            }
            value |= (uint)(b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                return true;
            }
            shift += 7;
        }
        throw new FormatException("Invalid Varint format");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryRead(ref SequenceReader<byte> reader, out int value)
    {
        if (!TryRead(ref reader, out uint zigzag))
        {
            value = 0;
            return false;
        }
        value = (int)(zigzag >> 1) ^ -(int)(zigzag & 1);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryRead(ref SequenceReader<byte> reader, out ulong value)
    {
        value = 0;
        var shift = 0;
        while (shift < 70) // 10 bytes max for 64-bit ulong
        {
            if (!reader.TryRead(out byte b))
            {
                return false;
            }
            value |= (ulong)(b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                return true;
            }
            shift += 7;
        }
        throw new FormatException("Invalid Varint format");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryRead(ref SequenceReader<byte> reader, out long value)
    {
        if (!TryRead(ref reader, out ulong zigzag))
        {
            value = 0;
            return false;
        }
        value = (long)(zigzag >> 1) ^ -(long)(zigzag & 1);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateByteLength(uint value)
    {
        var bytes = 1;
        while (value >= 0x80)
        {
            value >>= 7;
            bytes++;
        }
        return bytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateByteLength(int value)
    {
        var zigzag = (uint)((value << 1) ^ (value >> 31));
        return CalculateByteLength(zigzag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateByteLength(ulong value)
    {
        var bytes = 1;
        while (value >= 0x80)
        {
            value >>= 7;
            bytes++;
        }
        return bytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateByteLength(long value)
    {
        var zigzag = (ulong)((value << 1) ^ (value >> 63));
        return CalculateByteLength(zigzag);
    }
}
