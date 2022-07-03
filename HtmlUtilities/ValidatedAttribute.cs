using System.Text;

namespace HtmlUtilities;

/// <summary>
/// A combination of <see cref="ValidatedAttributeName"/> and <see cref="ValidatedAttributeValue"/>.
/// </summary>
public readonly struct ValidatedAttribute
{
    internal readonly byte[] value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided validated name and value.
    /// </summary>
    /// <param name="name">A validated name.</param>
    /// <param name="value">A validated value.</param>
    public ValidatedAttribute(ValidatedAttributeName name, ValidatedAttributeValue value)
    {
        var vv = value.value;
        if (vv.Length == 0)
        {
            this.value = name.value;
            return;
        }

        var nv = name.value;
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
    public ValidatedAttribute(string name, string? value) : this(new ValidatedAttributeName(name), new ValidatedAttributeValue(value))
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided unvalidated name and no value.
    /// </summary>
    /// <param name="name">A name to be validated.</param>
    public ValidatedAttribute(string name) : this(new ValidatedAttributeName(name))
    {
    }

    /// <summary>
    /// Returns a string of this attribute as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => Encoding.UTF8.GetString(this.value);
}
