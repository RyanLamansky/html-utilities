using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Enumerates <see cref="Rune"/>s from a <see cref="ReadOnlySpan{T}"/> of type <see cref="byte"/> without allocating heap memory.
/// </summary>
public ref struct Utf8DecoderEnumerator
{
    private ReadOnlySpan<byte>.Enumerator enumerator;

    /// <summary>
    /// The current <see cref="Rune"/> value. Not valid until <see cref="MoveNext"/> has been called at least once.
    /// </summary>
    public Rune Current { readonly get; private set; }

    internal Utf8DecoderEnumerator(ReadOnlySpan<byte> source)
    {
        this.enumerator = source.GetEnumerator();
        this.Current = default;
    }

    /// <summary>
    /// Reads the next <see cref="Rune"/> from the source.
    /// </summary>
    /// <returns>True if a value was found, false if the end of the source has been reached.</returns>
    public bool MoveNext()
    {
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current <= 0x7f)
            {
                Current = new(current);
                return true;
            }

            static bool Next(ReadOnlySpan<byte>.Enumerator enumerator, ref byte current)
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

                Current = new((b1 << 6) | current & 0b00111111);
            }
            else if ((current >> 4) == 0b1110)
            {
                b1 = current & 0b00001111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b2 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                Current = new((b1 << 12) | (b2 << 6) | current & 0b00111111);
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

                Current = new((b1 << 18) | (b2 << 12) | (b3 << 6) | current & 0b00111111);
            }

            return true;
        }

        return false;
    }
}
