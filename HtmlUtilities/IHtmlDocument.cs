using HtmlUtilities.Validated;

namespace HtmlUtilities;

/// <summary>
/// Automatically handles many of the standard features of an HTML document.
/// </summary>
public interface IHtmlDocument
{
    /// <summary>
    /// The IETF language tag of the document's language.
    /// By default, this attribute is not emitted.
    /// </summary>
    ValidatedAttributeValue Language => new();

    /// <summary>
    /// The document's title.
    /// By default, this attribute is not emitted.
    /// </summary>
    ValidatedText Title => new();

    /// <summary>
    /// A description of the contents of the document.
    /// By default, this is not emitted.
    /// </summary>
    ValidatedAttributeValue Description => new();

    /// <summary>
    /// Writes the content of an HTML document's body.
    /// </summary>
    /// <param name="writer">Receives the write commands.</param>
    /// <param name="cancellationToken">Indicates that the document is no longer needed so processing can be cancelled.</param>
    /// <returns>A task that, upon completion, indicates document writing is complete.</returns>
    /// <remarks>By default, directs the viewer to https://github.com/RyanLamansky/html-utilities to learn how to use this function.</remarks>
    Task WriteBodyContentsAsync(HtmlWriter writer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));

        writer.WriteElement("p", null, children =>
        {
            writer.WriteText("Visit ");
            writer.WriteElement("a", writer =>
            {
                writer.Write("href", "https://github.com/RyanLamansky/html-utilities");
            },
            writer =>
            {
                writer.WriteText("https://github.com/RyanLamansky/html-utilities");
            });
            writer.WriteText(" to learn how to customize an HtmlDocument instance.");
        });

        return Task.CompletedTask;
    }
}
