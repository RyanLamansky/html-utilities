using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// Enables pre-validation of HTML elements, optionally including attributes.
/// </summary>
public readonly struct ValidatedElement : IEquatable<ValidatedElement>
{
    internal readonly ReadOnlyMemory<byte> start;
    internal readonly ReadOnlyMemory<byte> end;

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> that contains no attributes.
    /// </summary>
    /// <param name="elementName">A validated element name.</param>
    public ValidatedElement(ValidatedElementName elementName)
        : this(elementName, [])
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> that contains no attributes.
    /// </summary>
    /// <param name="name">An element name to be validated and used.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElement(ReadOnlySpan<char> name)
        : this(new ValidatedElementName(name), [])
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> that contains no attributes.
    /// </summary>
    /// <param name="name">A UTF-8  element name to be validated and used.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElement(ReadOnlySpan<byte> name)
        : this(new ValidatedElementName(name), [])
    {
    }

    // Internal fast path for known-safe tag pairs.
    internal ValidatedElement(ReadOnlyMemory<byte> start, ReadOnlyMemory<byte> end)
    {
        this.start = start;
        this.end = end;
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> including attributes.
    /// </summary>
    /// <param name="name">A validated element name.</param>
    /// <param name="attributes">Optionally, validated attributes to include in the start tag.</param>
    public ValidatedElement(ValidatedElementName name, params ReadOnlySpan<ValidatedAttribute> attributes)
    {
        var elementNameValue = name.value;
        var attributeValueLengthSum = 0;
        foreach (var attribute in attributes)
            attributeValueLengthSum += attribute.value.Length;

        var buffer = new byte[elementNameValue.Length + attributeValueLengthSum + 2];
        buffer[0] = (byte)'<';
        elementNameValue.CopyTo(buffer.AsMemory(1));

        var written = elementNameValue.Length + 1;
        foreach (var attribute in attributes)
        {
            var value = attribute.value;
            value.CopyTo(buffer.AsMemory(written));
            written += value.Length;
        }

        buffer[^1] = (byte)'>';

        this.start = buffer;

        buffer = new byte[elementNameValue.Length + 3];
        buffer[0] = (byte)'<';
        buffer[1] = (byte)'/';
        elementNameValue.CopyTo(buffer.AsMemory(2));
        buffer[^1] = (byte)'>';

        this.end = buffer;
    }

    /// <summary>
    /// Returns the element start tag in string form.
    /// </summary>
    /// <returns>A string representation of the element start tag.</returns>
    public override string ToString() => Encoding.UTF8.GetString(start);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(start.GetContentHashCode(), end.GetContentHashCode());

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ValidatedElement value && Equals(value);

    /// <inheritdoc/>
    public bool Equals(ValidatedElement other) => this.start.ContentsEqual(other.start) && this.end.ContentsEqual(other.end);

    /// <summary>
    /// Determines whether two instances have the same content.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if their contents match, otherwise false.</returns>
    public static bool operator ==(ValidatedElement left, ValidatedElement right) => left.Equals(right);

    /// <summary>
    /// Determines whether two instances have the same contents.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>False if their contents match, otherwise true.</returns>
    public static bool operator !=(ValidatedElement left, ValidatedElement right) => !(left == right);
}
