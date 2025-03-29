using Microsoft.AspNetCore.Http;

namespace HtmlUtilities;

/// <summary>
/// Provides extended functionality for <see cref="IHtmlDocument"/> implementations.
/// </summary>
public static class HtmlDocumentExtensions
{
    /// <summary>
    /// Writes the document to <paramref name="context"/>.
    /// </summary>
    /// <param name="document">The document to write.</param>
    /// <param name="context">Receives the written document.</param>
    /// <returns>A task that shows completion when the document is written.</returns>
    public static ValueTask WriteToAsync(this IHtmlDocument document, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(context);

        var request = context.Request;
        var response = context.Response;
        response.ContentType = "text/html; charset=utf-8";
        Span<char> cspNonceUtf16 = stackalloc char[32];
        System.Security.Cryptography.RandomNumberGenerator.GetHexString(cspNonceUtf16, true);
        response.Headers.ContentSecurityPolicy = $"base-uri {request.Scheme}://{request.Host}/;default-src 'unsafe-inline' 'nonce-{cspNonceUtf16}' connect-src: 'self'";
        // unsafe-inline only applies to browsers that don't support nonce. Can be removed when security scanners stop asking for it.

        var writer = context.Response.BodyWriter;

        writer.Write("<!DOCTYPE html><html"u8);

        ReadOnlyMemory<byte> validated;

        if (!(validated = document.Language.value).IsEmpty)
        {
            writer.Write(" lang"u8);
            writer.Write(validated);
        }

        writer.Write("><head><meta charset=utf-8><meta name=viewport content=\"width=device-width, initial-scale=1\">"u8);

        if (!(validated = document.Title.value).IsEmpty)
        {
            writer.Write("<title>"u8);
            writer.Write(validated);
            writer.Write("</title>"u8);
        }

        if (!(validated = document.Description.value).IsEmpty)
        {
            writer.Write("<meta name=description content"u8);
            writer.Write(validated);
            writer.Write(">"u8);
        }

        var htmlWriter = new HtmlWriter(writer, new Validated.ValidatedAttribute("nonce", cspNonceUtf16));

        foreach (var link in document.Links ?? [])
            link.Write(htmlWriter, null, null);

        foreach (var style in document.Styles ?? [])
            style.Write(htmlWriter, null, null);

        writer.Write("</head><body>"u8);

        return document.WriteBodyContentsAsync(htmlWriter, context.RequestAborted);

        // HTML5 spec doesn't require </body></html>, so that simplifies things a bit here.
    }
}
