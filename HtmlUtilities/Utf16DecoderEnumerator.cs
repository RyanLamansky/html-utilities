using System.Runtime.CompilerServices;
using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Enumerates <see cref="Rune"/>s from a <see cref="ReadOnlySpan{T}"/> of type <see cref="char"/> without allocating heap memory.
/// </summary>
public ref struct Utf16DecoderEnumerator
{
    private ReadOnlySpan<char>.Enumerator enumerator;

    /// <summary>
    /// The current <see cref="Rune"/> value. Not valid until <see cref="MoveNext"/> has been called at least once.
    /// </summary>
    public Rune Current { readonly get; private set; }

    internal Utf16DecoderEnumerator(ReadOnlySpan<char> source)
    {
        this.enumerator = source.GetEnumerator();
        this.Current = default;
    }

    /// <summary>
    /// Reads the next <see cref="Rune"/> from the source.
    /// </summary>
    /// <returns>True if a value was found, false if the end of the source has been reached.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (!enumerator.MoveNext())
            return false;

        var high = (int)enumerator.Current;

        if (high <= 0xD7FF || (high >= 0xE000 && high <= 0xFFFF))
        {
            Current = new(high);
            return true;
        }
        else if (!enumerator.MoveNext())
        {
            return false;
        }

        var low = (int)enumerator.Current;
        Current = new((high - 0xD800) * 0x400 + (low - 0xDC00) + 0x10000);

        return true;
    }
}
