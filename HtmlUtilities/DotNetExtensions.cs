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
}
