using System.Buffers;
using System.IO.Pipelines;

namespace HtmlUtilities;

using Validated;

/// <summary>
/// Ensures a consistent degree of efficiency for the <see cref="HtmlWriter"/> over time.
/// </summary>
public static class HtmlWriterEfficiencyTests
{
    [Fact]
    public static void WriteDocumentEmptyEfficiency()
    {
        var writer = new TrackingPipeWriter();

        HtmlWriter.WriteDocument(writer);

        Assert.Equal(2, writer.Events.OfType<TrackingPipeWriter.AdvanceEvent>().Count());
    }

    [Fact]
    public static void WriteDocumentWithValidatedAttributeEfficiency()
    {
        var writer = new TrackingPipeWriter();

        HtmlWriter.WriteDocument(writer, attributes => attributes.Write(new ValidatedAttribute("lang", "en-us")));

        Assert.Equal(4, writer.Events.OfType<TrackingPipeWriter.AdvanceEvent>().Count());
    }

    [Fact]
    public static void WriteDocumentWithAttributeEfficiency()
    {
        var writer = new TrackingPipeWriter();

        HtmlWriter.WriteDocument(writer, attributes => attributes.Write("lang", "en-us"));

        Assert.Equal(4, writer.Events.OfType<TrackingPipeWriter.AdvanceEvent>().Count());
    }
}
