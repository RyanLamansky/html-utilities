using Microsoft.AspNetCore.Http;

namespace HtmlUtilities;

/// <summary>
/// Provides values for specific areas of an HTML document using asynchronous APIs.
/// </summary>
public interface IHtmlDocumentAsync
{
    /// <summary>
    /// Gets the Content-Security-Policy header value for the document.
    /// The default option is strict by contemporary standards.
    /// </summary>
    /// <param name="request">The request that will be receiving the policy in its response.</param>
    /// <param name="nonce">A cryptographically generated nonce encoded in hexadecimal.</param>
    /// <returns></returns>
    string GetContentSecurityPolicy(HttpRequest request, ReadOnlySpan<char> nonce)
    {
        ArgumentNullException.ThrowIfNull(request);

        return $"base-uri {request.Scheme}://{request.Host}/; default-src 'self'; script-src 'nonce-{nonce}' 'strict-dynamic' https: 'unsafe-inline'; style-src 'nonce-{nonce}'; object-src 'none' ";
    }

    /// <summary>
    /// Writes attributes to the outer HTML element.
    /// </summary>
    /// <param name="attributes">Receives the attributes.</param>
    /// <param name="cancellationToken">When triggered, the operation should be aborted.</param>
    /// <returns>Indicates completion of the operation.</returns>
    ValueTask WriteHtmlAttributesAsync(AttributeWriter attributes, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    /// <summary>
    /// Writes attributes to the head element.
    /// </summary>
    /// <param name="attributes">Receives the attributes.</param>
    /// <param name="cancellationToken">When triggered, the operation should be aborted.</param>
    /// <returns>Indicates completion of the operation.</returns>
    ValueTask WriteHeadAttributesAsync(AttributeWriter attributes, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    /// <summary>
    /// Writes children do the head element.
    /// </summary>
    /// <param name="children">Receives the child elements.</param>
    /// <param name="cancellationToken">When triggered, the operation should be aborted.</param>
    /// <returns>Indicates completion of the operation.</returns>
    ValueTask WriteHeadChildrenAsync(HtmlWriter children, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    /// <summary>
    /// Writes attributes to the body element.
    /// </summary>
    /// <param name="attributes">Receives the attributes.</param>
    /// <param name="cancellationToken">When triggered, the operation should be aborted.</param>
    /// <returns>Indicates completion of the operation.</returns>
    ValueTask WriteBodyAttributesAsync(AttributeWriter attributes, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    /// <summary>
    /// Writes children do the head element.
    /// </summary>
    /// <param name="children">Receives the child elements.</param>
    /// <param name="cancellationToken">When triggered, the operation should be aborted.</param>
    /// <returns>Indicates completion of the operation.</returns>
    ValueTask WriteBodyChildrenAsync(HtmlWriter children, CancellationToken cancellationToken) => ValueTask.CompletedTask;
}
