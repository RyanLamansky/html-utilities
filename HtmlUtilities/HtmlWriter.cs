﻿using System.Buffers;
using System.Text;

namespace HtmlUtilities;

/// <summary>
/// A high-performance writer for HTML5 content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public readonly ref struct HtmlWriter
{
    private static readonly byte[] doctype = Encoding.UTF8.GetBytes("<!DOCTYPE html>");
    private static readonly ValidatedElement html = new("html");

    private readonly IBufferWriter<byte> writer;

    private HtmlWriter(IBufferWriter<byte> writer)
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
    public static void WriteDocument(IBufferWriter<byte> writer, WriteAttributesCallback? attributes = null, WriteHtmlCallback? children = null)
    {
        new HtmlWriter(writer).WriteElement(html, attributes, children);
    }

    /// <summary>
    /// Writes a validated tag element.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    public void WriteElement(ValidatedElement element)
    {
        var writer = this.writer;

        writer.Write(element.start);
        writer.Write(element.end);
    }

    /// <summary>
    /// Writes a validated element with optional attributes and child content.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
    /// <param name="children">If provided, writes child elements.</param>
    public void WriteElement(ValidatedElement element, WriteAttributesCallback? attributes = null, WriteHtmlCallback? children = null)
    {
        var writer = this.writer;

        if (attributes is null)
            writer.Write(element.start);
        else
        {
            writer.Write(element.start.AsSpan(0, element.start.Length - 1));

            attributes(new AttributeWriter(this.writer));

            Span<byte> chars = stackalloc byte[1];
            chars[0] = (byte)'>';
            writer.Write(chars);
        }

        if (children is not null)
            children(this);

        writer.Write(element.end);
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    public void WriteElementSelfClosing(ValidatedElement element)
    {
        this.writer.Write(element.start);
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    public void WriteElementSelfClosing(ValidatedElement element, WriteAttributesCallback? attributes = null)
    {
        var writer = this.writer;

        if (attributes is null)
            writer.Write(element.start);
        else
        {
            writer.Write(element.start.AsSpan(0, element.start.Length - 1));

            attributes(new AttributeWriter(this.writer));

            Span<byte> chars = stackalloc byte[1];
            chars[0] = (byte)'>';
            writer.Write(chars);
        }
    }
}