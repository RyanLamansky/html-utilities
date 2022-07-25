using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// Writes attributes for an HTML element.
/// </summary>
public readonly struct AttributeWriter
{
    private readonly IBufferWriter<byte> writer;

    internal AttributeWriter(IBufferWriter<byte> writer)
    {
        System.Diagnostics.Debug.Assert(writer is not null);

        this.writer = writer;
    }

    /// <summary>
    /// Writes a validated attribute.
    /// </summary>
    /// <param name="attribute">The attribute to write.</param>
    public void Write(ValidatedAttribute attribute)
    {
        var value = attribute.value;
        if (value is null)
            return;

        writer.Write(value);
    }

    /// <summary>
    /// Writes an attribute that consists only of a name, no value.
    /// </summary>
    /// <param name="name">The attribute to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> cannot be null.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(string name)
    {
        var w = new ArrayBuilder<byte>(name.Length);
        try
        {
            ValidatedAttributeName.Validate(name, ref w);
            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes an attribute name and value pair.
    /// </summary>
    /// <param name="name">The attribute to write.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> cannot be null.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(string name, string? value)
    {
        var w = new ArrayBuilder<byte>(name.Length + (value?.Length).GetValueOrDefault() + 3);
        try
        {
            ValidatedAttributeName.Validate(name, ref w);

            if (value is not null)
                ValidatedAttributeValue.Validate(value, ref w);

            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }
}
