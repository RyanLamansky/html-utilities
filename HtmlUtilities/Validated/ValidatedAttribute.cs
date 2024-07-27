using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// A combination of <see cref="ValidatedAttributeName"/> and <see cref="ValidatedAttributeValue"/>.
/// </summary>
#pragma warning disable CA1711 // "Attribute" has a well known meaning in the context of HTML.
public readonly struct ValidatedAttribute
#pragma warning restore
{
    internal readonly byte[]? value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided validated name and value.
    /// </summary>
    /// <param name="name">A validated name.</param>
    /// <param name="value">A validated value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public ValidatedAttribute(ValidatedAttributeName name, ValidatedAttributeValue value)
    {
        var nv = name.value ?? throw new ArgumentException("name was never initialized.", nameof(name));
        var vv = value.value;
        if (vv is null)
        {
            this.value = nv;
            return;
        }

        var v = this.value = new byte[nv.Length + vv.Length];

        Array.Copy(nv, v, nv.Length);
        Array.Copy(vv, 0, v, nv.Length, vv.Length);
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
    /// Returns a string of this attribute as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => value is null ? "" : Encoding.UTF8.GetString(value);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and no value.
    /// </summary>
    /// <param name="name">A name to be validated.</param>
    public static implicit operator ValidatedAttribute(string name) => new(name);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and no value.
    /// </summary>
    /// <param name="name">A name to be validated.</param>
    public static implicit operator ValidatedAttribute(ReadOnlySpan<char> name) => new(name);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and value.
    /// </summary>
    /// <param name="nameAndValue">A name and value to be validated.</param>
    public static implicit operator ValidatedAttribute(KeyValuePair<string, string?> nameAndValue) => new(nameAndValue.Key, nameAndValue.Value);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and value.
    /// </summary>
    /// <param name="nameAndValue">A name and value to be validated.</param>
    public static implicit operator ValidatedAttribute((string Name, string? Value) nameAndValue) => new(nameAndValue.Name, nameAndValue.Value);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and value.
    /// </summary>
    /// <param name="nameAndValue">A name and value to be validated.</param>
    public static implicit operator ValidatedAttribute((string Name, int Value) nameAndValue) => new(new ValidatedAttributeName(nameAndValue.Name), new ValidatedAttributeValue(nameAndValue.Value));
}
