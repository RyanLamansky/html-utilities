namespace HtmlUtilities;

/// <summary>
/// Receives an <see cref="HtmlWriter"/> to write elements to a parent element.
/// </summary>
/// <param name="writer">The HTML element writer.</param>
public delegate void WriteHtmlCallback(HtmlWriter writer);

/// <summary>
/// Receives an <see cref="AttributeWriter"/> to write attibutes to a parent element.
/// </summary>
/// <param name="writer">The attribute writer.</param>
public delegate void WriteAttributesCallback(AttributeWriter writer);
