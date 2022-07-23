using System.Buffers;
using System.Text;

namespace HtmlUtilities;

public static class HtmlWriterAsyncTests
{
    [Fact]
    public static async Task EmptyDocument()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteChildElement()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, children: (writer, cancellationToken) => writer.WriteAsync(new ValidatedElement("head"), cancellationToken: cancellationToken)).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteUnconstructedChildElementThrows()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var array = new ValidatedElement[1];

        var caught = await Assert
            .ThrowsAsync<ArgumentException>(() => HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) => writer.WriteAsync(array[0], cancellationToken: cancellationToken)))
            .ConfigureAwait(false);

        Assert.Equal("element", caught.ParamName);
    }

    [Fact]
    public static async Task WriteAttribute()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, attributes => attributes.Write("lang", "en-us")).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html lang=en-us></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteAttributeAndChildElement()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, attributes => attributes.Write("lang", "en-us"), (writer, cancellationToken) => writer.WriteAsync(new ValidatedElement("head"), cancellationToken: cancellationToken)).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html lang=en-us><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosing()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                writer.WriteSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        }).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosingInBody()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteAsync(new ValidatedElement("body"), children: (writer, cancellationToken) =>
            {
                writer.WriteSelfClosing(new ValidatedElement("p"));
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        }).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html><body><p></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithMixedAttributes()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteAsync(new ValidatedElement("body"), children: (writer, cancellationToken) =>
            {
                return writer.WriteAsync(new ValidatedElement("div", new ValidatedAttribute[] { ("id", "react-app") }), attributes => attributes.Write("class", "root"), cancellationToken: cancellationToken);
            }, cancellationToken: cancellationToken);
        }).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html><body><div id=react-app class=root></div></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithText()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                return writer.WriteAsync(new ValidatedElement("title"), null, (writer, cancellationToken) =>
                {
                    writer.Write(new ValidatedText("Test"));
                    return Task.CompletedTask;
                }, cancellationToken);
            }, cancellationToken: cancellationToken);
        }).ConfigureAwait(false);

        Assert.Equal("<!DOCTYPE html><html><head><title>Test</title></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }
}
