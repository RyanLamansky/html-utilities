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

    internal sealed override void Write(HtmlWriter writer, Action<AttributeWriter>? dynamicAttributes, Action<HtmlWriter>? children)
    {
        writer.WriteElementRaw("<a>"u8, attributes =>
        {
            WriteGlobalAttributes(attributes);
            attributes.Write(writer.cspNonce);

            attributes.WriteRaw(" href"u8, Href);

            dynamicAttributes?.Invoke(attributes);
        }, children);
    }
}
