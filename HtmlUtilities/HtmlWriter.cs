using System.Buffers;
using System.Text;

namespace HtmlUtilities;

/// <summary>
/// A high-performance writer for HTML content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public class HtmlWriter
{
    internal static readonly byte[] doctype = Encoding.UTF8.GetBytes("<!DOCTYPE html>");
    internal static readonly ValidatedElement html = new("html");

    private protected readonly IBufferWriter<byte> writer;

    internal HtmlWriter(IBufferWriter<byte> writer)
    {
        ArgumentNullException.ThrowIfNull(this.writer = writer, nameof(writer));

        writer.Write(doctype);
    }

    /// <summary>
    /// Writes an HTML document using the provided buffer writer and callbacks.
    /// </summary>
    /// <param name="writer">Receives the written bytes.</param>
    /// <param name="attributes">If provided, writes attributes to the root HTML element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> cannot be null.</exception>
    public static void WriteDocument(IBufferWriter<byte> writer, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        new HtmlWriter(writer).Write(html, attributes, children);
    }

    /// <summary>
    /// Writes an HTML document using the provided buffer writer and callbacks.
    /// </summary>
    /// <param name="writer">Receives the written bytes.</param>
    /// <param name="attributes">If provided, writes attributes to the root HTML element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> cannot be null.</exception>
    public static Task WriteDocumentAsync(IBufferWriter<byte> writer, Action<AttributeWriter>? attributes = null, Func<HtmlWriterAsync, Task>? children = null)
    {
        return new HtmlWriterAsync(writer).WriteAsync(html, attributes, children);
    }

    /// <summary>
    /// Writes a validated tag element.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    public void Write(ValidatedElement element)
    {
        var writer = this.writer;

        writer.Write(element.start);
        writer.Write(element.end);
    }

    private protected static void WriteGreaterThan(IBufferWriter<byte> writer)
    {
        var chars = writer.GetSpan(1);
        chars[0] = (byte)'>';
        writer.Advance(1);
    }

    /// <summary>
    /// Writes a validated element with optional attributes and child content.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
    /// <param name="children">If provided, writes child elements.</param>
    public void Write(ValidatedElement element, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        var writer = this.writer;

        if (attributes is null)
            writer.Write(element.start);
        else
        {
            writer.Write(element.start.AsSpan(0, element.start.Length - 1));

            attributes(new AttributeWriter(this.writer));

            WriteGreaterThan(writer);
        }

        if (children is not null)
            children(this);

        writer.Write(element.end);
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    public void WriteSelfClosing(ValidatedElement element)
    {
        this.writer.Write(element.start);
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    public void WriteSelfClosing(ValidatedElement element, Action<AttributeWriter>? attributes = null)
    {
        var writer = this.writer;

        if (attributes is null)
            writer.Write(element.start);
        else
        {
            writer.Write(element.start.AsSpan(0, element.start.Length - 1));

            attributes(new AttributeWriter(this.writer));

            WriteGreaterThan(writer);
        }
    }

    /// <summary>
    /// Writes validated text.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void Write(ValidatedText text)
    {
        this.writer.Write(text.value);
    }
}