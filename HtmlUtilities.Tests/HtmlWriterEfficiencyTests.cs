using System.Buffers;

namespace HtmlUtilities;

using Validated;

/// <summary>
/// Ensures a consistent degree of efficiency for the <see cref="HtmlWriter"/> over time.
/// </summary>
public static class HtmlWriterEfficiencyTests
{
    /// <summary>
    /// Tracking calls to <see cref="IBufferWriter{T}.Advance(int)"/> is the most precise way to measure efficiency.
    /// There is overhead with each call, so the fewer the better.
    /// Tests built around the count of advances may see a reduction over time from optimization, but never regression.
    /// </summary>
    private sealed class AdvanceCounter : List<int>, IBufferWriter<byte>
    {
        private readonly byte[] buffer = new byte[256]; // This should be big enough for the test cases to keep things simple.

        void IBufferWriter<byte>.Advance(int count) => Add(count);

        Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint) => buffer;

        Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint) => buffer;
    }

    [Fact]
    public static void WriteDocumentEmptyEfficiency()
    {
        var counter = new AdvanceCounter();

        HtmlWriter.WriteDocument(counter);

        Assert.Equal(2, counter.Count);
    }

    [Fact]
    public static void WriteDocumentWithValidatedAttributeEfficiency()
    {
        var counter = new AdvanceCounter();

        HtmlWriter.WriteDocument(counter, attributes => attributes.Write(new ValidatedAttribute("lang", "en-us")));

        Assert.Equal(4, counter.Count);
    }

    [Fact]
    public static void WriteDocumentWithAttributeEfficiency()
    {
        var counter = new AdvanceCounter();

        HtmlWriter.WriteDocument(counter, attributes => attributes.Write("lang", "en-us"));

        Assert.Equal(4, counter.Count);
    }
}
