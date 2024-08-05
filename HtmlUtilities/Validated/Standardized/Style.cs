namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// Source: https://html.spec.whatwg.org/#the-style-element
/// </summary>
public class Style : StandardElement
{
    /// <summary>
    /// Text that gives a conformant style sheet.
    /// </summary>
    public ValidatedText Content { get; set; }

    /// <summary>
    /// Applicable media.
    /// </summary>
    public ValidatedAttributeValue? Media { get => GetAttribute(); set => SetAttribute(value); }

    internal sealed override void Write(HtmlWriter writer)
    {
        writer.WriteElement("<style>"u8, attributes =>
        {
            Write(attributes);
            attributes.Write(writer.cspNonce);

            attributes.Write(" media"u8, Media);
        }, children => children.WriteText(Content));
    }
}
