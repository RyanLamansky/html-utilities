namespace HtmlUtilities;

/// <summary>
/// The categories of a <see cref="System.Text.Rune"/> as defined by <a href="https://infra.spec.whatwg.org/#code-points">the "infra" standard</a>.
/// </summary>
[Flags]
public enum CodePointInfraCategory
{
    /// <summary>
    /// Invalid--outside the range of 0 through 0x10FFFF (1114111 in decimal).
    /// </summary>
    None = 0,
    /// <summary>
    /// A surrogate is a code point that is in the range U+D800 to U+DFFF, inclusive.
    /// </summary>
    Surrogate = 1 << 0,
    /// <summary>
    /// A scalar value is a code point that is not a <see cref="Surrogate"/>.
    /// </summary>
    ScalarValue = 1 << 1,
    /// <summary>
    /// A noncharacter is a code point with one of numerous defined values, see the spec for the complete list.
    /// </summary>
    NonCharacter = 1 << 2,
    /// <summary>
    /// An ASCII code point is a code point in the range U+0000 NULL to U+007F DELETE, inclusive.
    /// </summary>
    Ascii = 1 << 3,
    /// <summary>
    /// An ASCII tab or newline is U+0009 TAB, U+000A LF, or U+000D CR.
    /// </summary>
    AsciiTabOrNewline = 1 << 4,
    /// <summary>
    /// ASCII whitespace is U+0009 TAB, U+000A LF, U+000C FF, U+000D CR, or U+0020 SPACE.
    /// </summary>
    AsciiWhitespace = 1 << 5,
    /// <summary>
    /// A C0 control is a code point in the range U+0000 NULL to U+001F INFORMATION SEPARATOR ONE, inclusive.
    /// </summary>
    C0Control = 1 << 6,
    /// <summary>
    /// A C0 control or space is a <see cref="C0Control"/> or U+0020 SPACE.
    /// </summary>
    C0ControlOrSpace = 1 << 8,
    /// <summary>
    /// A control is a <see cref="C0Control"/> or a code point in the range U+007F DELETE to U+009F APPLICATION PROGRAM COMMAND, inclusive.
    /// </summary>
    Control = 1 << 9,
    /// <summary>
    /// An ASCII digit is a code point in the range U+0030 (0) to U+0039 (9), inclusive.
    /// </summary>
    AsciiDigit = 1 << 10,
    /// <summary>
    /// An ASCII upper hex digit is an <see cref="AsciiDigit"/> or a code point in the range U+0041 (A) to U+0046 (F), inclusive.
    /// </summary>
    AsciiUpperHexDigit = 1 << 11,
    /// <summary>
    /// An ASCII lower hex digit is an <see cref="AsciiDigit"/> or a code point in the range U+0061 (a) to U+0066 (f), inclusive.
    /// </summary>
    AsciiLowerHexDigit = 1 << 12,
    /// <summary>
    /// An ASCII hex digit is an <see cref="AsciiUpperHexDigit"/> or <see cref="AsciiLowerHexDigit"/>.
    /// </summary>
    AsciiHexDigit = 1 << 13,
    /// <summary>
    /// An ASCII upper alpha is a code point in the range U+0041 (A) to U+005A (Z), inclusive.
    /// </summary>
    AsciiUpperAlpha = 1 << 14,
    /// <summary>
    /// An ASCII lower alpha is a code point in the range U+0061 (a) to U+007A (z), inclusive.
    /// </summary>
    AsciiLowerAlpha = 1 << 15,
    /// <summary>
    /// An ASCII alpha is an <see cref="AsciiUpperAlpha"/> or <see cref="AsciiLowerAlpha"/>.
    /// </summary>
    AsciiAlpha = 1 << 16,
    /// <summary>
    /// An ASCII alphanumeric is an <see cref="AsciiDigit"/> or <see cref="AsciiAlpha"/>.
    /// </summary>
    AsciiAlphanumeric = 1 << 17,
}
