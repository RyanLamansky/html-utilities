using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Wraps a <see cref="ReadOnlySpan{T}"/> of type <see cref="char"/> for on-demand enumeration into <see cref="Rune"/>s.
/// </summary>
public readonly ref struct Utf16DecoderEnumerable
{
    private readonly ReadOnlySpan<char> source;

    internal Utf16DecoderEnumerable(ReadOnlySpan<char> source)
    {
        this.source = source;
    }

    /// <summary>
    /// Gets an enumerator to produce <see cref="Rune"/> from the source.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public Utf16DecoderEnumerator GetEnumerator() => new(source);
}
