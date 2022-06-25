using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// Writes attributes for an HTML element.
/// </summary>
public readonly ref struct AttributeWriter
{
    private readonly IBufferWriter<byte> writer;

    internal AttributeWriter(IBufferWriter<byte> writer)
    {
        System.Diagnostics.Debug.Assert(writer is not null);

        this.writer = writer;
    }

    /// <summary>
    /// Writes an attribute without a value.
    /// </summary>
    /// <param name="name">The attribute to write.</param>
    public void Write(ValidatedAttributeName name)
    {
        var writer = this.writer;

        writer.Write(" "u8);
        writer.Write(name.value);
    }
}
