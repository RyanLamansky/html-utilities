using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// A combination of <see cref="ValidatedAttributeName"/> and <see cref="ValidatedAttributeValue"/>.
/// </summary>
#pragma warning disable CA1711 // "Attribute" has a well known meaning in the context of HTML.
public readonly struct ValidatedAttribute
#pragma warning restore
{
    internal readonly ReadOnlyMemory<byte> value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided validated name and value.
    /// </summary>
    /// <param name="name">A validated name.</param>
    /// <param name="value">A validated value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public ValidatedAttribute(ValidatedAttributeName name, ValidatedAttributeValue value)
    {
        var nv = name.value;
        var vv = value.value;

        var v = new byte[nv.Length + vv.Length];

        nv.CopyTo(v);
        vv.CopyTo(v.AsMemory(nv.Length));

        this.value = v;
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided validated name and no value.
    /// </summary>
    /// <param name="name">A validated name.</param>
    public ValidatedAttribute(ValidatedAttributeName name)
    {
        this.value = name.value;
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and value.
    /// </summary>
    /// <param name="name">A name to be validated.</param>
    /// <param name="value">A value to be validated.</param>
    public ValidatedAttribute(ReadOnlySpan<char> name, ReadOnlySpan<char> value) : this(new ValidatedAttributeName(name), new ValidatedAttributeValue(value))
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and no value.
    /// </summary>
    /// <param name="name">A name to be validated.</param>
    public ValidatedAttribute(ReadOnlySpan<char> name) : this(new ValidatedAttributeName(name))
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and value.
    /// </summary>
    /// <param name="name">A UTF-8 name to be validated.</param>
    /// <param name="value">A UTF-8 value to be validated.</param>
    public ValidatedAttribute(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value) : this(new ValidatedAttributeName(name), new ValidatedAttributeValue(value))
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and no value.
    /// </summary>
    /// <param name="name">A UTF-8 name to be validated.</param>
    public ValidatedAttribute(ReadOnlySpan<byte> name) : this(new ValidatedAttributeName(name))
    {
    }

    /// <summary>
    /// Returns a string of this attribute as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => Encoding.UTF8.GetString(value);
}
