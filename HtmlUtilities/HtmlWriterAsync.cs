using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace HtmlUtilities;

/// <summary>
/// A high-performance asynchronous writer for HTML content.
/// </summary>
/// <remarks>UTF-8 is always used.</remarks>
public sealed class HtmlWriterAsync : HtmlWriter
{
    internal HtmlWriterAsync(IBufferWriter<byte> writer) : base(writer)
    {
    }

    /// <summary>
    /// Writes a validated element with optional attributes and child content.
    /// </summary>
    /// <param name="element">The validated HTML element.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
    /// <param name="children">If provided, writes child elements.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the operation.</returns>
    /// <exception cref="OperationCanceledException">A cancellation token in the call tree was triggered.</exception>
    /// <exception cref="ArgumentException"><paramref name="element"/> was never initialized.</exception>
    public async Task WriteElementAsync(
        ValidatedElement element,
        Action<AttributeWriter>? attributes = null,
        Func<HtmlWriterAsync, CancellationToken, Task>? children = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var writer = this.writer;
        var start = element.start;
        var end = element.end;

        if (start is null || end is null)
            throw new ArgumentException("element was never initialized.", nameof(element));

        if (attributes is null)
            writer.Write(start);
        else
        {
            writer.Write(start.AsSpan(0, start.Length - 1));

            attributes(new AttributeWriter(this.writer));

            WriteGreaterThan(writer);
        }

        if (children is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await children(this, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
        }

        writer.Write(end);
    }

    /// <summary>
    /// Writes an element with optional attributes and child content.
    /// If there are no children (for example, <paramref name="children"/> is null or writes nothing), use <see cref="HtmlWriter.WriteElement(string, Action{AttributeWriter}?, Action{HtmlWriter}?)"/> instead.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
    /// <param name="children">
    /// Writes child elements.
    /// This parameter is technically optional to provide symmetry with <see cref="HtmlWriter.WriteElement(string, Action{AttributeWriter}?, Action{HtmlWriter}?)"/>.
    /// However, this method should not be used without children--it will run synchronously by forwarding to the non-async version of this API.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the operation.</returns>
    /// <exception cref="OperationCanceledException">A cancellation token in the call tree was triggered.</exception>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public Task WriteElementAsync(
        string name,
        Action<AttributeWriter>? attributes = null,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [DisallowNull] Func<HtmlWriterAsync, CancellationToken, Task>? children = null,
#pragma warning restore
        CancellationToken cancellationToken = default)
        => WriteElementAsync(name.AsSpan(), attributes, children, cancellationToken);

    /// <summary>
    /// Writes an element with optional attributes and child content.
    /// If there are no children (for example, <paramref name="children"/> is null or writes nothing), use <see cref="HtmlWriter.WriteElement(ReadOnlySpan{char}, Action{AttributeWriter}?, Action{HtmlWriter}?)"/> instead.
    /// </summary>
    /// <param name="name">The HTML element name.</param>
    /// <param name="attributes">If provided, writes attributes to the element. Elements baked into the start tag are always included.</param>
    /// <param name="children">
    /// Writes child elements.
    /// This parameter is technically optional to provide symmetry with <see cref="HtmlWriter.WriteElement(ReadOnlySpan{char}, Action{AttributeWriter}?, Action{HtmlWriter}?)"/>.
    /// However, this method should not be used without children--it will run synchronously by forwarding to the non-async version of this API.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the operation.</returns>
    /// <exception cref="OperationCanceledException">A cancellation token in the call tree was triggered.</exception>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public Task WriteElementAsync(
        ReadOnlySpan<char> name,
        Action<AttributeWriter>? attributes = null,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [DisallowNull] Func<HtmlWriterAsync, CancellationToken, Task>? children = null,
#pragma warning restore
        CancellationToken cancellationToken = default)
    {
        if (children is null) // DisallowNull discourages people from using this API this way, but we'll still support it.
        {
            this.WriteElement(name, attributes, null);
            return Task.CompletedTask;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var writer = this.writer;

        var elementNameWriter = new ArrayBuilder<byte>(name.Length);

        ValidatedElementName.Validate(name, ref elementNameWriter);
        var elementNameBuffer = elementNameWriter.Buffer;
        var validatedName = elementNameWriter.WrittenMemory;

        WriteLessThan(writer);
        writer.Write(validatedName.Span);

        if (attributes is not null)
            attributes(new AttributeWriter(writer));

        WriteGreaterThan(writer);

        static async Task WriteElementAsync(
            HtmlWriterAsync htmlWriterAsync,
            ReadOnlyMemory<byte> validatedName,
            byte[] elementNameBuffer,
            Func<HtmlWriterAsync, CancellationToken, Task> children,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await children(htmlWriterAsync, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                var writer = htmlWriterAsync.writer;
                writer.Write(new[] { (byte)'<', (byte)'/' });
                writer.Write(validatedName.Span);
                WriteGreaterThan(writer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(elementNameBuffer);
            }
        }

        return WriteElementAsync(this, validatedName, elementNameBuffer, children, cancellationToken);
    }
}
