using System;

namespace HtmlUtilities;

/// <summary>
/// See https://html.spec.whatwg.org/multipage/named-characters.html for a complete list of options, though <see cref="HtmlWriter"/> only uses a select few.
/// </summary>
/// <remarks>The runtime is optimized so that the const-byte-array-to-readonlyspan pattern doesn't allocate any memory; see https://github.com/dotnet/roslyn/pull/24621.</remarks>
internal static class NamedCharacterReferences
{
    public static ReadOnlySpan<byte> Ampersand => new byte[] { (byte)'&', (byte)'a', (byte)'m', (byte)'p', (byte)';', };

    public static ReadOnlySpan<byte> LessThan => new byte[] { (byte)'&', (byte)'l', (byte)'t', (byte)';', };

    public static ReadOnlySpan<byte> Quote => new[] { (byte)'&', (byte)'q', (byte)'u', (byte)'o', (byte)'t', (byte)';', };
}
