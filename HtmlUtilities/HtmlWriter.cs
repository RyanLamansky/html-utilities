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
        new HtmlWriter(writer).Write(html, attributes, children);
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
        return new HtmlWriterAsync(writer).WriteAsync(html, attributes, children, cancellationToken);
    }

    /// <summary>
    /// Writes a validated tag element.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void Write(ValidatedElement element)
    {
        var writer = this.writer;
        var start = element.start;
        var end = element.end;

        if (start is null || end is null)
            throw new ArgumentException("element was never initialized.", nameof(element));

        writer.Write(start);
        writer.Write(end);
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
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void Write(ValidatedElement element, Action<AttributeWriter>? attributes = null, Action<HtmlWriter>? children = null)
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
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void WriteSelfClosing(ValidatedElement element)
    {
        var start = element.start;

        if (start is null)
            throw new ArgumentException("element was never initialized.", nameof(element));

        this.writer.Write(start);
    }

    /// <summary>
    /// Writes a validated element without an end tag.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public void WriteSelfClosing(ValidatedElement element, Action<AttributeWriter>? attributes = null)
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
    /// Writes validated text.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void Write(ValidatedText text)
    {
        var value = text.value;
        if (value is null)
            return;

        this.writer.Write(value);
    }
}