using System.IO.Pipelines;

namespace HtmlUtilities;

using System.Diagnostics;
using Validated;

/// <summary>
/// A high-performance writer for HTML content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public sealed class HtmlWriter
{
    private static readonly ValidatedElement html = new(
        "<!DOCTYPE html><html>"u8.ToArray(),
        "</html>"u8.ToArray());

    private readonly PipeWriter writer;
    internal readonly ValidatedAttribute cspNonce;

    internal HtmlWriter(PipeWriter writer)
        : this(writer, new ValidatedAttribute())
    {
    }

    internal HtmlWriter(PipeWriter writer, ValidatedAttribute cspNonce)
    {
        ArgumentNullException.ThrowIfNull(this.writer = writer, nameof(writer));
        this.cspNonce = cspNonce;
    }

    /// <summary>
    /// Pushes buffered written bytes to the client.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.</param>
    /// <returns>A task that represents and wraps the asynchronous flush operation.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was triggered.</exception>
    public ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default) => writer.FlushAsync(cancellationToken);

    /// <summary>
    /// Writes an HTML document using the provided buffer writer and callbacks.
    /// </summary>
    /// <param name="writer">Receives the written bytes.</param>
    /// <param name="attributes">If provided, writes attributes to the root HTML element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> cannot be null.</exception>
    public static void WriteDocument(PipeWriter writer, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        new HtmlWriter(writer).WriteElement(html, attributes, children);
    }

    /// <summary>
    /// Writes a validated element with no additional attributes or children.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void WriteElement(ValidatedElement element)
    {
        var writer = this.writer;
        var start = element.start;
        var end = element.end;

        if (start.IsEmpty)
            throw new ArgumentException("element was never initialized.", nameof(element));

        writer.Write(start);
        writer.Write(end);
    }

    /// <summary>
    /// Writes a standardized element with no additional attributes or children.
    /// </summary>
    /// <param name="element">The standardized HTML element.</param>
    public void WriteElement(StandardElement? element) => element?.Write(this, null);

    /// <summary>
    /// Writes a standardized element with no additional attributes or children.
    /// </summary>
    /// <param name="element">The standardized HTML element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    public void WriteElement(StandardElement? element, Action<HtmlWriter>? children) => WriteElement(element, null, children);

    /// <summary>
    /// Writes a standardized element with no additional attributes or children.
    /// </summary>
    /// <param name="element">The standardized HTML element.</param>
    /// <param name="dynamicAttributes">If provided, writes dynamic attributes after any that are set on the instance.</param>
    /// <param name="children">If provided, writes child elements.</param>
    public void WriteElement(
        StandardElement? element,
        Action<AttributeWriter>? dynamicAttributes = null,
        Action<HtmlWriter>? children = null) => element?.Write(this, dynamicAttributes, children);

    /// <summary>
    /// Writes an element with no attributes or children.
    /// </summary>
    /// <param name="element">The HTML element.</param>
    public void WriteElement(ReadOnlySpan<char> element)
    {
        var elementNameWriter = new ArrayBuilder<byte>(element.Length);
        var w = new ArrayBuilder<byte>(element.Length * 2);

        try
        {
            ValidatedElementName.Validate(element, ref elementNameWriter);
            var validatedElement = elementNameWriter.WrittenSpan;

            w.Write('<');
            w.Write(validatedElement);
            w.Write("></"u8);
            w.Write(validatedElement);
            w.Write('>');

            this.writer.Write(w.WrittenSpan);
        }
        finally
        {
            elementNameWriter.Release();
            w.Release();
        }
    }

    private static void WriteGreaterThan(PipeWriter writer)
    {
        var chars = writer.GetSpan();
        chars[0] = (byte)'>';
        writer.Advance(1);
    }

    private static void WriteLessThan(PipeWriter writer)
    {
        var chars = writer.GetSpan();
        chars[0] = (byte)'<';
        writer.Advance(1);
    }

    internal void WriteElementRaw(ReadOnlySpan<byte> nameWithAngleBrackets, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        Debug.Assert(nameWithAngleBrackets.Length >= 3 && nameWithAngleBrackets[0] == '<' && nameWithAngleBrackets[^1] == '>');

        var writer = this.writer;
        var start = nameWithAngleBrackets;
        Span<byte> end = stackalloc byte[nameWithAngleBrackets.Length + 1];
        end[0] = (byte)'<';
        end[1] = (byte)'/';
        nameWithAngleBrackets[1..].CopyTo(end[2..]);

        if (attributes is null)
            writer.Write(start);
        else
        {
            writer.Write(start[..^1]);

            attributes(new AttributeWriter(this.writer));

            WriteGreaterThan(writer);
        }

        if (children is not null)
            children(this);

        writer.Write(end);
    }

    /// <summary>
    /// Writes a validated element with optional attributes and child content.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Attributes baked into <paramref name="element"/> are always included.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void WriteElement(ValidatedElement element, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        var writer = this.writer;
        var start = element.start;
        var end = element.end;

        if (start.IsEmpty)
            throw new ArgumentException("element was never initialized.", nameof(element));

        if (attributes is null)
            writer.Write(start);
        else
        {
            writer.Write(start[..^1]);

            attributes(new AttributeWriter(this.writer));

            WriteGreaterThan(writer);
        }

        if (children is not null)
            children(this);

        writer.Write(end);
    }

    /// <summary>
    /// Writes an element with optional attributes and child content.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public void WriteElement(ReadOnlySpan<char> name, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        var elementNameWriter = new ArrayBuilder<byte>(name.Length);
        var writer = this.writer;

        try
        {
            ValidatedElementName.Validate(name, ref elementNameWriter);
            var validatedElement = elementNameWriter.WrittenSpan;

            WriteLessThan(writer);
            writer.Write(validatedElement);

            if (attributes is not null)
                attributes(new AttributeWriter(writer));

            WriteGreaterThan(writer);

            if (children is not null)
                children(this);

            writer.Write("</"u8);
            writer.Write(validatedElement);
        }
        finally
        {
            elementNameWriter.Release();
        }

        WriteGreaterThan(writer);
    }

    /// <summary>
    /// Writes an element with optional attributes and child content.
    /// </summary>
    /// <param name="name">The UTF-8 HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public void WriteElement(ReadOnlySpan<byte> name, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        var elementNameWriter = new ArrayBuilder<byte>(name.Length);
        var writer = this.writer;

        try
        {
            ValidatedElementName.Validate(name, ref elementNameWriter);
            var validatedElement = elementNameWriter.WrittenSpan;

            WriteLessThan(writer);
            writer.Write(validatedElement);

            if (attributes is not null)
                attributes(new AttributeWriter(writer));

            WriteGreaterThan(writer);

            if (children is not null)
                children(this);

            writer.Write("</"u8);
            writer.Write(validatedElement);
        }
        finally
        {
            elementNameWriter.Release();
        }

        WriteGreaterThan(writer);
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void WriteElementSelfClosing(ValidatedElement element)
    {
        var start = element.start;
        if (start.IsEmpty)
            throw new ArgumentException("element was never initialized.", nameof(element));

        this.writer.Write(start);
    }

    /// <summary>
    /// Writes an element without an end tag.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public void WriteElementSelfClosing(ReadOnlySpan<char> name)
    {
        var w = new ArrayBuilder<byte>(name.Length);

        try
        {
            w.Write('<');
            ValidatedElementName.Validate(name, ref w);
            w.Write('>');

            this.writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void WriteElementSelfClosing(ValidatedElement element, Action<AttributeWriter>? attributes = null)
    {
        var writer = this.writer;
        var start = element.start;
        if (start.IsEmpty)
            throw new ArgumentException("element was never initialized.", nameof(element));

        if (attributes is null)
            writer.Write(start);
        else
        {
            writer.Write(start[..^1]);

            attributes(new AttributeWriter(this.writer));

            WriteGreaterThan(writer);
        }
    }

    internal void WriteElementSelfClosing(ReadOnlySpan<byte> nameWithAngleBrackets, Action<AttributeWriter>? attributes = null)
    {
        System.Diagnostics.Debug.Assert(nameWithAngleBrackets[0] == (byte)'<' && nameWithAngleBrackets[nameWithAngleBrackets.Length - 1] == (byte)'>');

        writer.Write(nameWithAngleBrackets[..^1]);

        if (attributes is not null)
            attributes(new AttributeWriter(writer));

        WriteGreaterThan(writer);
    }

    /// <summary>
    /// Writes an element without an end tag.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public void WriteElementSelfClosing(ReadOnlySpan<char> name, Action<AttributeWriter>? attributes = null)
    {
        var w = new ArrayBuilder<byte>(name.Length);

        var writer = this.writer;

        try
        {
            w.Write('<');
            ValidatedElementName.Validate(name, ref w);

            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }

        if (attributes is not null)
            attributes(new AttributeWriter(writer));

        WriteGreaterThan(writer);
    }

    /// <summary>
    /// Writes validated text.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void WriteText(ValidatedText text)
        => this.writer.Write(text.value);

    /// <summary>
    /// Writes text.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void WriteText(ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
            return;

        var w = new ArrayBuilder<byte>(text.Length);
        try
        {
            ValidatedText.Validate(text, ref w);
            this.writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes text.
    /// </summary>
    /// <param name="text">The UTF-8 text to write.</param>
    public void WriteText(ReadOnlySpan<byte> text)
    {
        if (text.IsEmpty)
            return;

        var w = new ArrayBuilder<byte>(text.Length);
        try
        {
            ValidatedText.Validate(text, ref w);
            this.writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes a pre-validated script element.
    /// </summary>
    /// <param name="script">The script element to write.</param>
    public void WriteScript(ValidatedScript script)
        => WriteScript(null, script);

    /// <summary>
    /// Writes a script element that uses attributes exclusively.
    /// </summary>
    /// <param name="attributes">Writes attributes to the element.</param>
    public void WriteScript(Action<AttributeWriter> attributes)
        => WriteScript(attributes, default);

    /// <summary>
    /// Writes a pre-validated script element.
    /// </summary>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <param name="script">The script element to write.</param>
    public void WriteScript(Action<AttributeWriter>? attributes, ValidatedScript script)
    {
        var value = script.value;
        writer.Write("<script"u8);
        writer.Write(this.cspNonce.value);
        if (attributes is not null)
            attributes(new AttributeWriter(writer));
        writer.Write(value);
        writer.Write("</script>"u8);
    }
}