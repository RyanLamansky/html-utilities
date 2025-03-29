namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// The HTML "noscript" element, from https://html.spec.whatwg.org/#the-noscript-element.
/// </summary>
public class NoScript : StandardElement
{
    internal sealed override void Write(HtmlWriter writer, Action<AttributeWriter>? dynamicAttributes, Action<HtmlWriter>? children)
    {
        writer.WriteElementRaw("<noscript>"u8, attributes =>
        {
            base.WriteGlobalAttributes(attributes);

            dynamicAttributes?.Invoke(attributes);
        }, children);
    }
}
