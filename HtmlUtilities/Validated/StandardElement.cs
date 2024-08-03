namespace HtmlUtilities.Validated;

/// <summary>
/// Provides friendly syntax for use of standardized HTML elements.
/// </summary>
public abstract class StandardElement
{
    // Global attribute list from https://html.spec.whatwg.org/#global-attributes.

    /// <summary>
    /// Uniquely identifies an element.
    /// Source: https://dom.spec.whatwg.org/#concept-id
    /// </summary>
    public ValidatedAttributeValue? Id { get; set; }

    /// <summary>
    /// The title attribute represents advisory information for the element, such as would be appropriate for a tooltip.
    /// Source: https://html.spec.whatwg.org/#attr-title
    /// </summary>
    public ValidatedAttributeValue? Title { get; set; }

    private protected StandardElement()
    {
    }

    internal abstract void Write(HtmlWriter writer);

    /// <summary>
    /// Writes global attributes to the provided <see cref="AttributeWriter"/>.
    /// </summary>
    /// <param name="writer">Receives the attributes.</param>
    private protected void Write(AttributeWriter writer)
    {
        writer.Write(" id"u8, Id);
        writer.Write(" title"u8, Title);
    }
}
