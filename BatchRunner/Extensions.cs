
namespace BatchRunner.Extensions;

using System.Collections;
using System.Data.Common;


public static class Extensions{
    /*
        Extract a byte treating the first bit as the LSB.

        Since we typically think of an array as having its index moving left to right, if we simply
        extracted the bits as a slice and thought of that as the byte, the first bit in the array
        would end up being most significant (MSB).

        Example conversion for bits 0000 0010:

            MSB on left: 2
            LSB on left: 64

        SeaMAX docs say the byte is treated as MSB -> LSB where MSB bits represent higher channels:

            byte value: 64
            seamax bits: 0100 0000
            seamax channels (zero indexed):
                channel 6: on
                other channels: off

        Therefore, we will want to reverse the order of bits before we pack the byte array and
        this method does that.
    */
    public static byte ExtractByteReversed(this BitArray bits, int offset)
    {
        byte b = 0;

        if (bits.Get(offset + 0)) b++;
        if (bits.Get(offset + 1)) b += 2;
        if (bits.Get(offset + 2)) b += 4;
        if (bits.Get(offset + 3)) b += 8;
        if (bits.Get(offset + 4)) b += 16;
        if (bits.Get(offset + 5)) b += 32;
        if (bits.Get(offset + 6)) b += 64;
        if (bits.Get(offset + 7)) b += 128;

        return b;
    }

    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> ie)
    {
        int i = 0;
        foreach (var item in ie) {
            yield return (i, item);
            i++;
        }
    }

    public static string GetString(this DbDataReader reader, string name) {
        return reader.GetString(reader.GetOrdinal(name));
    }

    public static int GetInt32(this DbDataReader reader, string name) {
        return reader.GetInt32(reader.GetOrdinal(name));
    }

    public static bool GetBoolean(this DbDataReader reader, string name) {
        return reader.GetBoolean(reader.GetOrdinal(name));
    }

    public static int? GetNInt32(this DbDataReader reader, string name) {
        var ord = reader.GetOrdinal(name);
        return reader.IsDBNull(ord) ? (int?)null : reader.GetInt32(ord);
    }

    public static T GetEnum<T>(this DbDataReader reader, string name) where T : Enum {
        int ordinal = reader.GetOrdinal(name);
        int value = reader.GetInt32(ordinal);
        return (T)Enum.ToObject(typeof(T), value);
    }




}


// static BitArray Reverse(this BitArray array)
// {
//     int length = array.Length;
//     int mid = (length / 2);

//     for (int i = 0; i < mid; i++)
//     {
//         bool bit = array[i];
//         array[i] = array[length - i - 1];
//         array[length - i - 1] = bit;
//     }

//     return new BitArray(array);
// }




