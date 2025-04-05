using HtmlUtilities.Validated;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO.Pipelines;

namespace HtmlUtilities;

/// <summary>
/// A high-performance writer for HTML content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public sealed class HtmlWriter
{
    private static readonly ValidatedElement html = new("<!DOCTYPE html><html>"u8, "</html>"u8);
    private static readonly ValidatedElement head = new("<head>"u8, "</head>"u8);
    private static readonly ValidatedElement body = new("<body>"u8, "</body>"u8);
    private static readonly ValidatedElement metaCharsetUtf8 = new("<meta charset=utf-9>"u8, ""u8);

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
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents and wraps the asynchronous flush operation.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was triggered.</exception>
    public ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken) => writer.FlushAsync(cancellationToken);

    /// <summary>
    /// Writes an HTML document using the provided writer and callbacks.
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
    /// Writes an HTML document using the provided context.
    /// </summary>
    /// <param name="context">Receives the written bytes.</param>
    /// <param name="document">The document to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> and <paramref name="document"/> cannot be null.</exception>
    public static void WriteDocument(HttpContext context, IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(document);

        var request = context.Request;
        var response = context.Response;
        response.ContentType = "text/html; charset=utf-8";
        Span<char> cspNonceUtf16 = stackalloc char[32];
        System.Security.Cryptography.RandomNumberGenerator.GetHexString(cspNonceUtf16, true);
        response.Headers.ContentSecurityPolicy = document.GetContentSecurityPolicy(request, cspNonceUtf16);

        var writer = new HtmlWriter(response.BodyWriter, new("nonce", cspNonceUtf16));
        writer.WriteElement(html, document.WriteHtmlAttributes, children =>
        {
            writer.WriteElement(head, document.WriteHeadAttributes, children =>
            {
                children.WriteElement(metaCharsetUtf8);
                document.WriteHeadChildren(writer);
            });

            writer.WriteElement(body, document.WriteBodyAttributes, document.WriteBodyChildren);
        });
    }

    /// <summary>
    /// Writes an HTML document using the provided context asynchronously.
    /// <see cref="HttpContext.RequestAborted"/> is used as a cancellation token.
    /// </summary>
    /// <param name="context">Receives the written bytes.</param>
    /// <param name="document">The document to write.</param>
    /// <returns>A task that represents and wraps the asynchronous document writing operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> and <paramref name="document"/> cannot be null.</exception>
    public static ValueTask WriteDocumentAsync(HttpContext context, IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(context);

        return WriteDocumentAsync(context, document, context.RequestAborted);
    }

    /// <summary>
    /// Writes an HTML document using the provided context asynchronously.
    /// </summary>
    /// <param name="context">Receives the written bytes.</param>
    /// <param name="document">The document to write.</param>
    /// <param name="cancellationToken">When triggered, the operation is cancelled.</param>
    /// <returns>A task that represents and wraps the asynchronous document writing operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> and <paramref name="document"/> cannot be null.</exception>
    public static async ValueTask WriteDocumentAsync(HttpContext context, IHtmlDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(document);

        var request = context.Request;
        var response = context.Response;
        response.ContentType = "text/html; charset=utf-8";
        Span<char> cspNonceUtf16 = stackalloc char[32];
        System.Security.Cryptography.RandomNumberGenerator.GetHexString(cspNonceUtf16, true);
        response.Headers.ContentSecurityPolicy = document.GetContentSecurityPolicy(request, cspNonceUtf16);

        var writer = new HtmlWriter(response.BodyWriter, new("nonce", cspNonceUtf16));
        await writer.WriteElementAsync(html, document.WriteHtmlAttributesAsync, async (children, cancellationToken) =>
        {
            await writer.WriteElementAsync(head, document.WriteHeadAttributesAsync, async (children, cancellationToken) =>
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression - IDE conflicts with code analysis.
#pragma warning disable CA1849 // Call async methods when in an async method - Not necessary for this code path.
                children.WriteElement(metaCharsetUtf8);
#pragma warning restore
                await document.WriteHeadChildrenAsync(writer, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            await writer
                .WriteElementAsync(body, document.WriteBodyAttributesAsync, document.WriteBodyChildrenAsync, cancellationToken)
                .ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
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
    /// Writes a validated element with optional attributes and child content.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Attributes baked into <paramref name="element"/> are always included.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <param name="cancellationToken">When triggered, the operation is cancelled.</param>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public async ValueTask WriteElementAsync(
        ValidatedElement element,
        Func<AttributeWriter, CancellationToken, ValueTask>? attributes = null,
        Func<HtmlWriter, CancellationToken, ValueTask>? children = null,
        CancellationToken cancellationToken = default)
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

            await attributes(new(this.writer), cancellationToken).ConfigureAwait(false);

            WriteGreaterThan(writer);
        }

        if (children is not null)
            await children(this, cancellationToken).ConfigureAwait(false);

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