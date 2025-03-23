namespace HtmlUtilities;

/// <summary>
/// Adds functionality I wish were included in the base .NET code.
/// </summary>
internal static class DotNetExtensions
{
    public static string GetString(this System.Text.Encoding encoding, ReadOnlyMemory<byte> bytes)
        => encoding.GetString(bytes.Span);

    public static void Write<T>(this System.Buffers.IBufferWriter<T> writer, ReadOnlyMemory<T> value)
        => System.Buffers.BuffersExtensions.Write(writer, value.Span);

    public static bool ContentsEqual<T>(this ReadOnlyMemory<T> left, ReadOnlyMemory<T> right)
        where T : IEquatable<T>
        => left.Span.ContentsAreEqual(right.Span);

    public static bool ContentsAreEqual<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        where T : IEquatable<T>
    {
        if (left == right)
            return true; // Both spans reference the same location in memory

        if (left.Length != right.Length)
            return false;

        for (var i = 0; i < left.Length; i++)
            if (!left[i].Equals(right[i]))
                return false;

        return true;
    }

    public static int GetContentHashCode<T>(this ReadOnlyMemory<T> source)
        where T : IEquatable<T>
        => source.Span.GetContentHashCode();

    public static int GetContentHashCode<T>(this ReadOnlySpan<T> source)
        where T : IEquatable<T>
    {
        var hash = new HashCode();

        foreach (var c in source)
            hash.Add(c.GetHashCode());

        return hash.ToHashCode();
    }
}
