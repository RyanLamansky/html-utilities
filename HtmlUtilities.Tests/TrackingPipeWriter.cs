using System.IO.Pipelines;

namespace HtmlUtilities;

internal sealed class TrackingPipeWriter : PipeWriter
{
    public abstract record PipeEvent;

    public sealed record AdvanceEvent(int Bytes) : PipeEvent;

    public sealed record CancelPendingFlushEvent() : PipeEvent;

    public sealed record CompleteEvent(Exception? Exception) : PipeEvent;

    public sealed record FlushEvent() : PipeEvent;

    public sealed record GetMemoryEvent(int SizeHint) : PipeEvent;

    public sealed record GetSpanEvent(int SizeHint) : PipeEvent;

    public List<PipeEvent> Events { get; } = [];

    private readonly MemoryPipeWriter writer = new();

    public override void Advance(int bytes)
    {
        Events.Add(new AdvanceEvent(bytes));
        writer.Advance(bytes);
    }

    public override void CancelPendingFlush()
    {
        Events.Add(new CancelPendingFlushEvent());
        writer.CancelPendingFlush();
    }

    public override void Complete(Exception? exception = null)
    {
        Events.Add(new CompleteEvent(exception));
        writer.Complete(exception);
    }

    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        Events.Add(new FlushEvent());
        return writer.FlushAsync(cancellationToken);
    }

    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        Events.Add(new GetMemoryEvent(sizeHint));
        return writer.GetMemory(sizeHint);
    }

    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        Events.Add(new GetSpanEvent(sizeHint));
        return writer.GetSpan();
    }

    public ReadOnlySpan<byte> WrittenSpan => writer.WrittenSpan;
}
