using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// A high-performance writer for HTML5 content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public readonly ref struct HtmlWriter
{
    private readonly IBufferWriter<byte> writer;

    private HtmlWriter(IBufferWriter<byte> writer)
    {
        ArgumentNullException.ThrowIfNull(this.writer = writer, nameof(writer));

        writer.Write("<!DOCTYPE html>"u8);
    }

    /// <summary>
    /// Writes an HTML document to the provided buffer writer and callbacks.
    /// </summary>
    /// <param name="writer">Receives the written bytes.</param>
    /// <param name="attributes">If provided, writes attributes to the root HTML element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> and <paramref name="children"/> cannot be null.</exception>
    public static void WriteDocument(IBufferWriter<byte> writer, WriteAttributesCallback? attributes = null, WriteHtmlCallback? children = null)
    {
        var htmlWriter = new HtmlWriter(writer);
        htmlWriter.WriteElement("html", attributes, children);
    }

    /// <summary>
    /// Writes a validated element.
    /// </summary>
    /// <param name="name">The validated HTML element name.</param>
    public void WriteElement(ValidatedElementName name)
    {
        var writer = this.writer;

        writer.Write("<"u8);
        writer.Write(name.value);
        writer.Write("></"u8);
        writer.Write(name.value);
        writer.Write(">"u8);
    }

    /// <summary>
    /// Validates and writes an element.
    /// </summary>
    /// <param name="name">The unvalidated HTML element name.</param>
    public void WriteElement(string name) => WriteElement(new ValidatedElementName(name));

    /// <summary>
    /// Writes a validated element with its associated attributes and children.
    /// </summary>
    /// <param name="name">The validated HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    public void WriteElement(ValidatedElementName name, WriteAttributesCallback? attributes = null, WriteHtmlCallback? children = null)
    {
        var writer = this.writer;
        writer.Write("<"u8);
        writer.Write(name.value);

        if (attributes is not null)
            attributes(new AttributeWriter(this.writer));

        writer.Write(">"u8);

        if (children is not null)
            children(this);

        writer.Write("</"u8);
        writer.Write(name.value);
        writer.Write(">"u8);
    }

    /// <summary>
    /// Writes a validated element with its associated attributes.
    /// </summary>
    /// <param name="name">The validated HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    public void WriteElement(ValidatedElementName name, WriteAttributesCallback? attributes)
        => WriteElement(name, attributes, null);

    /// <summary>
    /// Validates and writes an element with its associated attributes.
    /// </summary>
    /// <param name="name">The unvalidated HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    public void WriteElement(string name, WriteAttributesCallback? attributes)
        => WriteElement(new ValidatedElementName(name), attributes);

    /// <summary>
    /// Validates and writes an element with its associated attributes and children.
    /// </summary>
    /// <param name="name">The unvalidated HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    public void WriteElement(string name, WriteAttributesCallback? attributes = null, WriteHtmlCallback? children = null)
        => WriteElement(new ValidatedElementName(name), attributes, children);

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="name">The validated HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    public void WriteElementSelfClosing(ValidatedElementName name, WriteAttributesCallback? attributes = null)
    {
        var writer = this.writer;

        writer.Write("<"u8);
        writer.Write(name.value);

        if (attributes is not null)
            attributes(new AttributeWriter(this.writer));

        writer.Write(">"u8);
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="name">The validated HTML element name.</param>
    public void WriteElementSelfClosing(ValidatedElementName name) => WriteElementSelfClosing(name, null);

    /// <summary>
    ///  Validates and writes an element without an end tag.
    /// </summary>
    /// <param name="name">The unvalidated HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    public void WriteElementSelfClosing(string name, WriteAttributesCallback? attributes = null) => WriteElementSelfClosing(new ValidatedElementName(name), attributes);

    /// <summary>
    ///  Validates and writes an element without an end tag.
    /// </summary>
    /// <param name="name">The unvalidated HTML element name.</param>
    public void WriteElementSelfClosing(string name) => WriteElementSelfClosing(new ValidatedElementName(name));
}