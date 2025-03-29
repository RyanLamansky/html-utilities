namespace HtmlUtilities;

#pragma warning disable IDE0079 // Remove unnecessary suppression - IDE disagrees with the compiler about CA1033.
#pragma warning disable CA1033 // Interface methods should be callable by child types
#pragma warning restore IDE0079 // Remove unnecessary suppression

/// <summary>
/// Provides values for specific areas of an HTML document using synchronous APIs.
/// This interface extends <see cref="IHtmlDocumentAsync"/> by forwarding asynchronous calls to the synchronous versions.
/// </summary>
public interface IHtmlDocument : IHtmlDocumentAsync
{
    /// <summary>
    /// Writes attributes to the outer HTML element.
    /// Not used if <see cref="IHtmlDocumentAsync.WriteHtmlAttributesAsync(AttributeWriter, CancellationToken)"/> is implemented.
    /// </summary>
    /// <param name="attributes">Receives the attributes.</param>
    void WriteHtmlAttributes(AttributeWriter attributes)
    {
    }

    ValueTask IHtmlDocumentAsync.WriteHtmlAttributesAsync(AttributeWriter attributes, CancellationToken cancellationToken)
    {
        WriteHtmlAttributes(attributes);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Writes attributes to the head element.
    /// Not used if <see cref="IHtmlDocumentAsync.WriteHeadAttributesAsync(AttributeWriter, CancellationToken)"/> is implemented.
    /// </summary>
    /// <param name="attributes">Receives the attributes.</param>
    void WriteHeadAttributes(AttributeWriter attributes)
    {
    }

    ValueTask IHtmlDocumentAsync.WriteHeadAttributesAsync(AttributeWriter attributes, CancellationToken cancellationToken)
    {
        WriteHeadAttributes(attributes);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Writes children do the head element.
    /// Not used if <see cref="IHtmlDocumentAsync.WriteHeadChildrenAsync(HtmlWriter, CancellationToken)"/> is implemented.
    /// </summary>
    /// <param name="children">Receives the child elements.</param>
    void WriteHeadChildren(HtmlWriter children)
    {
    }

    ValueTask IHtmlDocumentAsync.WriteHeadChildrenAsync(HtmlWriter children, CancellationToken cancellationToken)
    {
        WriteHeadChildren(children);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Writes attributes to the body element.
    /// Not used if <see cref="IHtmlDocumentAsync.WriteBodyAttributesAsync(AttributeWriter, CancellationToken)"/> is implemented.
    /// </summary>
    /// <param name="attributes">Receives the attributes.</param>
    void WriteBodyAttributes(AttributeWriter attributes)
    {
    }

    ValueTask IHtmlDocumentAsync.WriteBodyAttributesAsync(AttributeWriter attributes, CancellationToken cancellationToken)
    {
        WriteBodyAttributes(attributes);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Writes children do the head element.
    /// Not used if <see cref="IHtmlDocumentAsync.WriteBodyChildrenAsync(HtmlWriter, CancellationToken)"/> is implemented.
    /// </summary>
    /// <param name="children">Receives the child elements.</param>
    void WriteBodyChildren(HtmlWriter children)
    {
    }

    ValueTask IHtmlDocumentAsync.WriteBodyChildrenAsync(HtmlWriter children, CancellationToken cancellationToken)
    {
        WriteBodyChildren(children);
        return ValueTask.CompletedTask;
    }
}
