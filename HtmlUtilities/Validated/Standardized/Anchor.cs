namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// The HTML "a" element, from https://html.spec.whatwg.org/#the-a-element.
/// </summary>
public class Anchor : StandardElement
{
    /// <summary>
    /// Address of the hyperlink.
    /// </summary>
    public ValidatedAttributeValue? Href { get => GetAttribute(); set => SetAttribute(value); }

    internal sealed override void Write(HtmlWriter writer)
    {
        writer.WriteElementRaw("<a>"u8, attributes =>
        {
            Write(attributes);
            attributes.Write(writer.cspNonce);

            attributes.WriteRaw(" href"u8, Href);
        });
    }
}
