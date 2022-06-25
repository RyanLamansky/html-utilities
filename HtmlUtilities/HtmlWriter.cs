using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// A high-performance writer for HTML5 content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public readonly ref struct HtmlWriter
{
    private readonly IBufferWriter<byte> writer;

    /// <summary>
    /// Creates a new <see cref="HtmlWriter"/> instance for the provided buffer writer.
    /// </summary>
    /// <param name="writer">Receives the written bytes.</param>
    public HtmlWriter(IBufferWriter<byte> writer)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        this.writer = writer;

        writer.Write("<!DOCTYPE html>"u8);
    }

    /// <summary>
    /// Writes an element.
    /// </summary>
    /// <param name="element">A validated HTML element.</param>
    public void WriteElement(ValidatedElementName element)
    {
        this.WriteElementCommon(element);
        
        var writer = this.writer;

        writer.Write("</"u8);
        writer.Write(element.value);
        writer.Write(">"u8);
    }

    /// <summary>
    /// Writes an element.
    /// </summary>
    /// <param name="element">A validated HTML element.</param>
    /// <param name="attributes">Writes attributes to the element.</param>
    public void WriteElement(ValidatedElementName element, WriteAttributesCallback attributes)
    {
        var writer = this.writer;
        writer.Write("<"u8);
        writer.Write(element.value);
        attributes(new AttributeWriter(this.writer));
        writer.Write(">"u8);

        writer.Write("</"u8);
        writer.Write(element.value);
        writer.Write(">"u8);
    }

    /// <summary>
    /// Writes an element without an end tag.
    /// </summary>
    /// <param name="element">A validated HTML element.</param>
    public void WriteElementSelfClosing(ValidatedElementName element)
    {
        this.WriteElementCommon(element);
    }

    private void WriteElementCommon(ValidatedElementName element)
    {
        var writer = this.writer;

        writer.Write("<"u8);
        writer.Write(element.value);
        writer.Write(">"u8);
    }
}