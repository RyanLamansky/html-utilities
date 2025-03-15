using System.Collections.Frozen;

namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// https://html.spec.whatwg.org/#the-script-element
/// </summary>
public class Script : StandardElement
{
    private static readonly FrozenSet<string?> JavaScriptMime = FrozenSet.ToFrozenSet([
        null,
        "",
        "application/ecmascript",
        "application/javascript",
        "application/x-ecmascript",
        "application/x-javascript",
        "text/ecmascript",
        "text/javascript",
        "text/javascript1.0",
        "text/javascript1.1",
        "text/javascript1.2",
        "text/javascript1.3",
        "text/javascript1.4",
        "text/javascript1.5",
        "text/jscript",
        "text/livescript",
        "text/x-ecmascript",
        "text/x-javascript",
        ], StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///Allows customization of the type of script represented.
    ///If the value is set to one of the JavaScript MIME types (see https://mimesniff.spec.whatwg.org/#javascript-mime-type), it is changed to "null".
    /// </summary>
    public ValidatedAttributeValue? Type
    {
        get => GetAttribute();
        //TODO: Revalidate Content upon set.
        set => SetAttribute(JavaScriptMime.Contains(value.ToString()) ? default : value);
    }

    private ValidatedText content;

    /// <summary>
    /// Text of the script. There are various conformance rules depending on the <see cref="Type"/>.
    /// </summary>
    public ValidatedText Content
    {
        get => this.content;
        set
        {
            // TODO: Revalidate Type upon set.
            // TODO: Validate all types described by https://html.spec.whatwg.org/#the-script-element

            if (value.IsEmpty)
            {
                this.content = value;
                return;
            }

            this.content = new ValidatedText(ValidatedScript.ForInlineSource(value.value.Span).value[1..]);
        }
    }

    internal sealed override void Write(HtmlWriter writer) => writer.WriteElementRaw("<script>"u8, attributes =>
    {
        attributes.WriteRaw(" type"u8, this.Type);
        base.Write(attributes);
        attributes.Write(writer.cspNonce);
    }, writer => writer.WriteText(this.content));
}
