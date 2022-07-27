using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// A high-performance writer for HTML content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public class HtmlWriter
{
    private static readonly ValidatedElement html = new(
        new byte[]
        {
            (byte)'<', (byte)'!', (byte)'D', (byte)'O', (byte)'C', (byte)'T', (byte)'Y', (byte)'P', (byte)'E', (byte)' ', (byte)'h', (byte)'t', (byte)'m', (byte)'l', (byte)'>',
            (byte)'<', (byte)'h', (byte)'t', (byte)'m', (byte)'l', (byte)'>',
        },
        new byte[]
        {
            (byte)'<', (byte)'/', (byte)'h', (byte)'t', (byte)'m', (byte)'l', (byte)'>',
        });

    private protected readonly IBufferWriter<byte> writer;

    internal HtmlWriter(IBufferWriter<byte> writer)
    {
        ArgumentNullException.ThrowIfNull(this.writer = writer, nameof(writer));
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
        new HtmlWriter(writer).WriteElement(html, attributes, children);
    }

    /// <summary>
    /// Writes an HTML document using the provided buffer writer and callbacks.
    /// </summary>
    /// <param name="writer">Receives the written bytes.</param>
    /// <param name="attributes">If provided, writes attributes to the root HTML element.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <param name="cancellationToken">Used to trigger cancellation of writing activity.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> cannot be null.</exception>
    /// <exception cref="OperationCanceledException">A cancellation token in the call tree was triggered.</exception>
    public static Task WriteDocumentAsync(
        IBufferWriter<byte> writer,
        Action<AttributeWriter>? attributes = null,
        Func<HtmlWriterAsync, CancellationToken, Task>? children = null,
        CancellationToken cancellationToken = default)
    {
        return new HtmlWriterAsync(writer).WriteElementAsync(html, attributes, children, cancellationToken);
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

        if (start is null || end is null)
            throw new ArgumentException("element was never initialized.", nameof(element));

        writer.Write(start);
        writer.Write(end);
    }

    /// <summary>
    /// Writes an element with no attributes or children.
    /// </summary>
    /// <param name="element">The HTML element.</param>
    public void WriteElement(string element) => WriteElement(element.AsSpan());

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

            w.Write((byte)'<');
            w.Write(validatedElement);
            w.Write(new [] { (byte)'>', (byte)'<', (byte)'/', });
            w.Write(validatedElement);
            w.Write((byte)'>');

            this.writer.Write(w.WrittenSpan);
        }
        finally
        {
            elementNameWriter.Release();
            w.Release();
        }
    }

    private protected static void WriteGreaterThan(IBufferWriter<byte> writer)
    {
        var chars = writer.GetSpan();
        chars[0] = (byte)'>';
        writer.Advance(1);
    }

    private protected static void WriteLessThan(IBufferWriter<byte> writer)
    {
        var chars = writer.GetSpan();
        chars[0] = (byte)'<';
        writer.Advance(1);
    }

    /// <summary>
    /// Writes a validated element with optional attributes and child content.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void WriteElement(ValidatedElement element, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
    {
        var writer = this.writer;
        var start = element.start;
        var end = element.end;

        if (start is null || end is null)
            throw new ArgumentException("element was never initialized.", nameof(element));

        if (attributes is null)
            writer.Write(start);
        else
        {
            writer.Write(element.start.AsSpan(0, start.Length - 1));

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
    /// <param name="name">The HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public void WriteElement(string name, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
        => WriteElement(name.AsSpan(), attributes, children);

    /// <summary>
    /// Writes an element with optional attributes and child content.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
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

            writer.Write(new[] { (byte)'<', (byte)'/' });
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

        if (start is null)
            throw new ArgumentException("element was never initialized.", nameof(element));

        this.writer.Write(start);
    }

    /// <summary>
    /// Writes an element without an end tag.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public void WriteElementSelfClosing(string name) => WriteElementSelfClosing(name.AsSpan());

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
            w.Write((byte)'<');
            ValidatedElementName.Validate(name, ref w);
            w.Write((byte)'>');

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

        if (start is null)
            throw new ArgumentException("element was never initialized.", nameof(element));

        if (attributes is null)
            writer.Write(start);
        else
        {
            writer.Write(element.start.AsSpan(0, start.Length - 1));

            attributes(new AttributeWriter(this.writer));

            WriteGreaterThan(writer);
        }
    }

    /// <summary>
    /// Writes an element without an end tag.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public void WriteElementSelfClosing(string name, Action<AttributeWriter>? attributes = null) => WriteElementSelfClosing(name.AsSpan(), attributes);

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
            w.Write((byte)'<');
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
    {
        var value = text.value;
        if (value is null)
            return;

        this.writer.Write(value);
    }

    /// <summary>
    /// Writes text.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void WriteText(string? text) => WriteText(text.AsSpan());

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
}