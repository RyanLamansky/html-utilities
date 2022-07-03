using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Enables pre-validation of HTML attribute names by storing only valid names.
/// </summary>
public readonly struct ValidatedAttributeName
{
    internal readonly byte[] value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> cannot be null.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public ValidatedAttributeName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        if (name.Length == 0)
            throw new ArgumentException("name cannot be an empty string.", nameof(name));

        this.value = CodePoint.EncodeUtf8(Validate(CodePoint.DecodeUtf16(name))).ToArray();
    }

    private static IEnumerable<CodePoint> Validate(IEnumerable<CodePoint> name)
    {
        // https://html.spec.whatwg.org/#attributes-2

        foreach (var cp in name)
        {
            switch (cp.Value)
            {
                default:
                    yield return cp;
                    continue;
                case '&':
                    yield return '&';
                    yield return 'a';
                    yield return 'm';
                    yield return 'p';
                    yield return ';';
                    continue;

                case >= 0x0000 and <= 0x001F: // C0 Control
                case >= 0x007F and <= 0x009F: // Control

                // Specific characters
                case ' ':
                case '"':
                case '\'':
                case '>':
                case '/':
                case '=':

                // Noncharacters 
                case >= 0xFDD0 and <= 0xFDEF:
                case 0xFFFE:
                case 0xFFFF:
                case 0x1FFFE:
                case 0x1FFFF:
                case 0x2FFFE:
                case 0x2FFFF:
                case 0x3FFFE:
                case 0x3FFFF:
                case 0x4FFFE:
                case 0x4FFFF:
                case 0x5FFFE:
                case 0x5FFFF:
                case 0x6FFFE:
                case 0x6FFFF:
                case 0x7FFFE:
                case 0x7FFFF:
                case 0x8FFFE:
                case 0x8FFFF:
                case 0x9FFFE:
                case 0x9FFFF:
                case 0xAFFFE:
                case 0xAFFFF:
                case 0xBFFFE:
                case 0xBFFFF:
                case 0xCFFFE:
                case 0xCFFFF:
                case 0xDFFFE:
                case 0xDFFFF:
                case 0xEFFFE:
                case 0xEFFFF:
                case 0xFFFFE:
                case 0xFFFFF:
                case 0x10FFFE:
                case 0x10FFFF:
                    throw new ArgumentException($"name has an invalid character, code point {cp.Value}", nameof(name));
            }
        }
    }

    /// <summary>
    /// Converts the validated name to a string.
    /// </summary>
    /// <returns>The string representation of the validated name.</returns>
    public override string ToString() => Encoding.UTF8.GetString(this.value);
}
