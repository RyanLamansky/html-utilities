using System.Runtime.CompilerServices;

namespace HtmlUtilities.Validated;

/// <summary>
/// Provides friendly syntax for use of standardized HTML elements.
/// </summary>
public abstract class StandardElement
{
    // Global attribute list from https://html.spec.whatwg.org/#global-attributes.

    private readonly Dictionary<string, ValidatedAttributeValue> attributes = [];

    private protected ValidatedAttributeValue? GetAttribute([CallerMemberName] string name = null!)
        => attributes.TryGetValue(name, out var value) ? value : null;

    private protected void SetAttribute(ValidatedAttributeValue? value, [CallerMemberName] string name = null!)
    {
        if (value is null)
            return;

        attributes[name] = value.GetValueOrDefault();
    }

    /// <summary>
    /// All HTML elements may have the accesskey content attribute set
    /// The accesskey attribute's value is used by the user agent as a guide for creating a keyboard shortcut that activates or focuses the element.
    /// If specified, the value must be an ordered set of unique space-separated tokens none of which are identical to another token and each of which must be exactly one code point in length.
    /// Source: https://html.spec.whatwg.org/#the-accesskey-attribute
    /// </summary>
    public ValidatedAttributeValue? AccessKey { get => GetAttribute(); set => SetAttribute(value); }

    /// <summary>
    /// Uniquely identifies an element.
    /// Source: https://dom.spec.whatwg.org/#concept-id
    /// </summary>
    public ValidatedAttributeValue? Id { get => GetAttribute(); set => SetAttribute(value); }

    /// <summary>
    /// The title attribute represents advisory information for the element, such as would be appropriate for a tooltip.
    /// Source: https://html.spec.whatwg.org/#attr-title
    /// </summary>
    public ValidatedAttributeValue? Title { get => GetAttribute(); set => SetAttribute(value); }

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
        writer.Write(" accesskey"u8, AccessKey);
        writer.Write(" id"u8, Id);
        writer.Write(" title"u8, Title);
    }
}
