using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// Provides extended functionality for <see cref="IHtmlDocument"/> implementations.
/// </summary>
public static class HtmlDocumentExtensions
{
    /// <summary>
    /// Writes the document to <paramref name="writer"/>.
    /// </summary>
    /// <param name="document">The document to write.</param>
    /// <param name="writer">Receives the written document.</param>
    /// <param name="cancellationToken">Cancels emission of document data.</param>
    /// <returns></returns>
    public static async Task WriteAsync(
        this IHtmlDocument document,
        IBufferWriter<byte> writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document, nameof(document));
        cancellationToken.ThrowIfCancellationRequested();

        writer.Write("<!DOCTYPE html><html"u8);

        byte[]? validated;

        if ((validated = document.Language.value) is not null)
        {
            writer.Write(" lang"u8);
            writer.Write(validated);
        }

        writer.Write("><head><meta charset=utf-8><meta name=viewport content=\"width=device-width, initial-scale=1\">"u8);

        if ((validated = document.Title.value) is not null)
        {
            writer.Write("<title>"u8);
            writer.Write(validated);
            writer.Write("</title>"u8);
        }

        if ((validated = document.Description.value) is not null)
        {
            writer.Write("<meta name=description content"u8);
            writer.Write(validated);
            writer.Write(">"u8);
        }

        writer.Write("</head><body>"u8);

        await document.WriteBodyContentsAsync(new HtmlWriter(writer), cancellationToken).ConfigureAwait(false);

        writer.Write("</body></html>"u8);
    }
}
