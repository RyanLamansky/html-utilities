namespace HtmlUtilities;

/// <summary>
/// Wraps a <see cref="ReadOnlySpan{T}"/> of type <see cref="char"/> for on-demand enumeration into <see cref="CodePoint"/>s.
/// </summary>
public readonly ref struct Utf8DecoderEnumerable
{
    private readonly ReadOnlySpan<byte> source;

    internal Utf8DecoderEnumerable(ReadOnlySpan<byte> source)
    {
        this.source = source;
    }

    /// <summary>
    /// Gets an enumerator to produce <see cref="CodePoint"/> from the source.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public Utf8DecoderEnumerator GetEnumerator() => new(source);
}
