using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public async Task WriteElementAsync(
        ValidatedElement element,
        Action<AttributeWriter>? attributes = null,
        Func<HtmlWriterAsync, Task>? children = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

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
        {
            cancellationToken.ThrowIfCancellationRequested();
            await children(this).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
        }

        writer.Write(element.end);
    }
}
