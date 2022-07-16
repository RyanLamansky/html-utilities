﻿using System.Buffers;

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
    public async Task WriteAsync(
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
}
