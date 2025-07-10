using System.Collections;

namespace Simulador.utils;

public static class BitArrayExtensions
{
    public static BitArray FromInt(int value, int length)
    {
        BitArray bits = new(length);
        for (int i = 0; i < length; i++)
        {
            bits[i] = (value & (1 << i)) != 0;
        }
        return bits;
    }

    public static BitArray FromBitString(string bitString, bool lmsb = true)
    {
        int length = bitString.Length;
        BitArray bits = new(length);

        for (int i = 0; i < length; i++)
        {
            char c = bitString[lmsb ? length - 1 - i : i];
            if (c == '1')
                bits[i] = true;
            else if (c == '0')
                bits[i] = false;
            else
                throw new ArgumentException("bitString must contain only '0's and '1's.");
        }

        return bits;
    }

    public static bool Compare(this BitArray a, BitArray b)
    {
        if (a == null || b == null) return false;
        if (a.Length != b.Length) return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }

        return true;
    }

    public static int ToInt32(this BitArray bits)
    {
        if (bits.Length > 32)
            throw new ArgumentException("BitArray length must be at most 32 bits.");

        int value = 0;
        for (int i = 0; i < bits.Length; i++)
        {
            if (bits[i]) value |= (1 << i);
        }
        return value;
    }

    public static int C2ToInt32(this BitArray bits)
    {
        return 0;
    }

    public static string ToBitString(this BitArray bits)
    {
        if (bits == null) return string.Empty;

        char[] chars = new char[bits.Length];
        for (int i = 0; i < bits.Length; i++)
        {
            chars[bits.Length - 1 - i] = bits[i] ? '1' : '0';
        }
        return new string(chars);
    }

    public static BitArray TrimOrPad(this BitArray source, int targetLength)
    {
        BitArray result = new(targetLength);
        int len = Math.Min(source.Length, targetLength);

        for (int i = 0; i < len; i++)
        {
            result[i] = source[i];
        }

        return result;
    }

    public static BitArray ShiftLeft(this BitArray input)
    {
        BitArray shifted = new(input.Length);
        for (int i = 0; i < input.Length - 1; i++)
            shifted[i] = input[i + 1];
        return shifted;
    }

    public static BitArray ShiftRight(this BitArray input)
    {
        BitArray shifted = new(input.Length);
        for (int i = 1; i < input.Length; i++)
            shifted[i] = input[i - 1];
        return shifted;
    }
}