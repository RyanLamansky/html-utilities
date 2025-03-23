using System.Buffers;
using System.IO.Pipelines;

namespace HtmlUtilities;

using Validated;

/// <summary>
/// Ensures a consistent degree of efficiency for the <see cref="HtmlWriter"/> over time.
/// </summary>
public static class HtmlWriterEfficiencyTests
{
    /// <summary>
    /// Tracking calls to <see cref="PipeWriter.Advance(int)"/> is the most precise way to measure efficiency.
    /// There is overhead with each call, so the fewer the better.
    /// Tests built around the count of advances may see a reduction over time from optimization, but never regression.
    /// </summary>
    private sealed class AdvanceCounter : PipeWriter
    {        
        private readonly ArrayBufferWriter<byte> writer = new();

        public int Count { get; private set; }

        public override void Advance(int bytes)
        {
            writer.Advance(bytes);
            Count++;
        }

        public override void CancelPendingFlush()
        {
        }

        public override void Complete(Exception? exception = null)
        {
        }

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
            => new(new FlushResult(false, true));

        public override Memory<byte> GetMemory(int sizeHint = 0) => writer.GetMemory(sizeHint);

        public override Span<byte> GetSpan(int sizeHint = 0) => writer.GetSpan(sizeHint);

        public ReadOnlySpan<byte> WrittenSpan => writer.WrittenSpan;
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
