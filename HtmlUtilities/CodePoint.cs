using System.Runtime.CompilerServices;

namespace HtmlUtilities;

/// <summary>
/// Converts data to or from Unicode code points.
/// </summary>
public static class CodePoint
{
    /// <summary>
    /// Decodes a sequence of UTF-8 bytes into Unicode code points. Invalid bytes are skipped.
    /// </summary>
    /// <param name="source">The sequence of bytes.</param>
    /// <returns>The sequence of code points.</returns>
    public static IEnumerable<int> DecodeUtf8(IEnumerable<byte> source)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            
            if (current <= 0x7f)
            {
                yield return current;
                continue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool Next(IEnumerator<byte> enumerator, ref byte current)
            {
                if (!enumerator.MoveNext())
                    return false; // Invalid sequence.
                if (((current = enumerator.Current) >> 6) != 0b10)
                    return false;

                return true;
            }

            int b1, b2, b3;
            if ((current >> 5) == 0b110)
            {
                b1 = current & 0b00011111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                yield return (b1 << 6) | current & 0b00111111;
            }
            else if ((current >> 4) == 0b1110)
            {
                b1 = current & 0b00001111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b2 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                yield return (b1 << 12) | (b2 << 6) | current & 0b00111111;
            }
            else if ((current >> 3) == 0b11110)
            {
                b1 = current & 0b00001111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b2 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b3 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                yield return (b1 << 18) | (b2 << 12) | (b3 << 6) | current & 0b00111111;
            }
        }
    }

    /// <summary>
    /// Encodes a sequence of Unicode code points into UTF-8 bytes. Invalid code points are skipped.
    /// </summary>
    /// <param name="source">The sequence of code points.</param>
    /// <returns>The sequence of bytes.</returns>
    public static IEnumerable<byte> EncodeUtf8(IEnumerable<int> source)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            
            if (current <= 0x7F)
                yield return (byte)current;
            else if (current <= 0x7FF)
            {
                yield return (byte)(0b11000000 | (current >> 6));
                yield return (byte)(0b10000000 | current & 0b00111111);
            }
            else if (current <= 0xFFFF)
            {
                yield return (byte)(0b11100000 | (current >> 12));
                yield return (byte)(0b10000000 | (current >> 6) & 0b00111111);
                yield return (byte)(0b10000000 | current & 0b00111111);
            }
            else if (current <= 0x10FFFF)
            {
                yield return (byte)(0b11110000 | (current >> 18));
                yield return (byte)(0b10000000 | (current >> 12) & 0b00111111);
                yield return (byte)(0b10000000 | (current >> 6) & 0b00111111);
                yield return (byte)(0b10000000 | current & 0b00111111);
            }
        }
    }
}
