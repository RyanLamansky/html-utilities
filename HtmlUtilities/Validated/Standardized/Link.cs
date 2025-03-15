namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// The HTML "link" element, from https://html.spec.whatwg.org/#the-link-element.
/// </summary>
public class Link : StandardElement
{
    /// <summary>
    /// Relationship between the document containing the hyperlink and the destination resource.
    /// </summary>
    public ValidatedAttributeValue? Rel { get => GetAttribute(); set => SetAttribute(value); }

    /// <summary>
    /// Address of the hyperlink.
    /// </summary>
    public ValidatedAttributeValue? Href { get => GetAttribute(); set => SetAttribute(value); }

    internal sealed override void Write(HtmlWriter writer)
    {
        writer.WriteElementSelfClosing("<link>"u8, attributes =>
        {
            base.Write(attributes);

            attributes.WriteRaw(" rel"u8, Rel);
            attributes.WriteRaw(" href"u8, Href);
        });
    }
}
