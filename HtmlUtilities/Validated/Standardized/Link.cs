namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// The HTML "link" element, from https://html.spec.whatwg.org/#the-link-element.
/// </summary>
public class Link : StandardElement
{
    /// <summary>
    /// Relationship between the document containing the hyperlink and the destination resource.
    /// </summary>
    public string? Rel { get; set; }

    /// <summary>
    /// Address of the hyperlink.
    /// </summary>
    public string? Href { get; set; }

    internal sealed override void Write(HtmlWriter writer)
    {
        writer.WriteElementSelfClosing("<link>"u8, attributes =>
        {
            base.Write(attributes);

            attributes.Write(" rel"u8, Rel);
            attributes.Write(" href"u8, Href);
        });
    }
}
