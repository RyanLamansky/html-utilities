using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// Writes attributes for an HTML element.
/// </summary>
public readonly ref struct AttributeWriter
{
    private readonly IBufferWriter<byte> writer;

    internal AttributeWriter(IBufferWriter<byte> writer)
    {
        System.Diagnostics.Debug.Assert(writer is not null);

        this.writer = writer;
    }

    /// <summary>
    /// Writes a validated attribute.
    /// </summary>
    /// <param name="attribute">The attribute to write.</param>
    public void Write(ValidatedAttribute attribute)
    {
        var writer = this.writer;

        writer.Write(" "u8);
        writer.Write(attribute.value);
    }

    /// <summary>
    /// Writes a validated attribute name and its corresponding value.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    /// <param name="value">The value to write.</param>
    public void Write(ValidatedAttributeName name, ValidatedAttributeValue value)
    {
        var writer = this.writer;

        writer.Write(" "u8);
        writer.Write(name.value);
        writer.Write(value.value);
    }

    /// <summary>
    /// Writes a validated attribute name with no value.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    public void Write(ValidatedAttributeName name)
    {
        var writer = this.writer;

        writer.Write(" "u8);
        writer.Write(name.value);
    }

    /// <summary>
    /// Writes an unvalidated attribute name and its corresponding value.
    /// </summary>
    /// <param name="name">The attribute name to verify and write.</param>
    /// <param name="value">The value to verify and write.</param>
    public void Write(string name, string? value) => this.Write(new ValidatedAttribute(name, value));

    /// <summary>
    /// Writes an unvalidated attribute name with no value.
    /// </summary>
    /// <param name="name">The attribute name to verify and write.</param>
    public void Write(string name) => this.Write(new ValidatedAttribute(name));
}
