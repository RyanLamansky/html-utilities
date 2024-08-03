using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// Enables pre-validation of HTML elements, optionally including attributes.
/// </summary>
public readonly struct ValidatedElement
{
    internal readonly ReadOnlyMemory<byte> start;
    internal readonly ReadOnlyMemory<byte> end;

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> that contains no attributes.
    /// </summary>
    /// <param name="elementName">A validated element name.</param>
    public ValidatedElement(ValidatedElementName elementName)
        : this(elementName, (IEnumerable<ValidatedAttribute>?)null)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> that contains no attributes.
    /// </summary>
    /// <param name="name">An element name to be validated and used.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElement(ReadOnlySpan<char> name)
        : this(new ValidatedElementName(name), (IEnumerable<ValidatedAttribute>?)null)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> including attributes.
    /// </summary>
    /// <param name="name">A validated element name.</param>
    /// <param name="attributes">Optionally, validated attributes to include in the start tag.</param>
    public ValidatedElement(ValidatedElementName name, params ValidatedAttribute[]? attributes)
        : this(name, (IEnumerable<ValidatedAttribute>?)attributes)
    {
    }

    // Internal fast path for known-safe tag pairs.
    internal ValidatedElement(byte[] start, byte[] end)
    {
        this.start = start;
        this.end = end;
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> including attributes.
    /// </summary>
    /// <param name="name">A validated element name.</param>
    /// <param name="attributes">Optionally, validated attributes to include in the start tag.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public ValidatedElement(ValidatedElementName name, IEnumerable<ValidatedAttribute>? attributes)
    {
        attributes ??= [];

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
    /// Creates a new <see cref="ValidatedElement"/> including attributes.
    /// </summary>
    /// <param name="name">An element name to be validated and used.</param>
    /// <param name="attributes">Optionally, validated attributes to include in the start tag.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElement(ReadOnlySpan<char> name, params ValidatedAttribute[]? attributes)
        : this(new ValidatedElementName(name), (IEnumerable<ValidatedAttribute>?)attributes)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElement"/> including attributes.
    /// </summary>
    /// <param name="name">An element name to be validated and used.</param>
    /// <param name="attributes">Optionally, validated attributes to include in the start tag.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElement(ReadOnlySpan<char> name, IEnumerable<ValidatedAttribute>? attributes)
        : this(new ValidatedElementName(name), attributes)
    {
    }

    /// <summary>
    /// Returns the element start tag in string form.
    /// </summary>
    /// <returns>A string representation of the element start tag.</returns>
    public override string ToString() => Encoding.UTF8.GetString(start);
}
