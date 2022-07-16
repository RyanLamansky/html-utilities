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

        var writer = this.writer;

        writer.Write(value);
    }
}
