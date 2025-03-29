namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// The HTML "br" element, from https://html.spec.whatwg.org/#the-br-element.
/// </summary>
public class LineBreak : StandardElement
{
    internal sealed override void Write(HtmlWriter writer)
    {
        writer.WriteElementSelfClosing("<br>"u8, base.Write);
    }
}
