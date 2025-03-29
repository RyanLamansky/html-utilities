using System.Buffers;
using System.IO.Pipelines;

namespace HtmlUtilities;

internal sealed class MemoryPipeWriter : PipeWriter
{
    private readonly ArrayBufferWriter<byte> writer = new();

    public override void Advance(int bytes) => writer.Advance(bytes);

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

    public ReadOnlyMemory<byte> WrittenMemory => writer.WrittenMemory;
}
