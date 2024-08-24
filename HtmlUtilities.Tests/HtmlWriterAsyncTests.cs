using System.Buffers;
using System.Text;

namespace HtmlUtilities;

using Validated;

public static class HtmlWriterAsyncTests
{
    [Fact]
    public static async Task EmptyDocument()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer);

        Assert.Equal("<!DOCTYPE html><html></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteChildElement()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, children: (writer, cancellationToken) => writer.WriteElementAsync(new ValidatedElement("head"), cancellationToken: cancellationToken));

        Assert.Equal("<!DOCTYPE html><html><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteUnconstructedChildElementThrows()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var array = new ValidatedElement[1];

        var caught = await Assert
            .ThrowsAsync<ArgumentException>(() => HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) => writer.WriteElementAsync(array[0], cancellationToken: cancellationToken)))
            ;

        Assert.Equal("element", caught.ParamName);
    }

    [Fact]
    public static async Task WriteAttribute()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, attributes => attributes.Write("lang", "en-us"));

        Assert.Equal("<!DOCTYPE html><html lang=en-us></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteAttributeAndChildElement()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, attributes => attributes.Write("lang", "en-us"), (writer, cancellationToken) => writer.WriteElementAsync(new ValidatedElement("head"), cancellationToken: cancellationToken));

        Assert.Equal("<!DOCTYPE html><html lang=en-us><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithValidatedElementAndTextChild()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                writer.WriteText("Test");
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head>Test</head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithStringElementAndTextChild()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync("head", children: (writer, cancellationToken) =>
            {
                writer.WriteText("Test");
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head>Test</head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithReadOnlySpanElementAndTextChild()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync("head".AsSpan(), children: (writer, cancellationToken) =>
            {
                writer.WriteText("Test");
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head>Test</head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosingValidated()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                writer.WriteElementSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosingString()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                writer.WriteElementSelfClosing("meta", attributes => attributes.Write("charset", "utf-8"));
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosing()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                writer.WriteElementSelfClosing("meta".AsSpan(), attributes => attributes.Write("charset", "utf-8"));
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosingValidatedInBody()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("body"), children: (writer, cancellationToken) =>
            {
                writer.WriteElementSelfClosing(new ValidatedElement("p"));
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><body><p></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosingStringInBody()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("body"), children: (writer, cancellationToken) =>
            {
                writer.WriteElementSelfClosing("p");
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><body><p></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithSelfClosingReadOnlySpanCharInBody()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("body"), children: (writer, cancellationToken) =>
            {
                writer.WriteElementSelfClosing("p".AsSpan());
                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><body><p></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithMixedAttributes()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("body"), children: (writer, cancellationToken) =>
            {
                return writer.WriteElementAsync(new ValidatedElement("div", [("id", "react-app")]), attributes => attributes.Write("class", "root"), cancellationToken: cancellationToken);
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><body><div id=react-app class=root></div></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithValidatedText()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                return writer.WriteElementAsync(new ValidatedElement("title"), null, (writer, cancellationToken) =>
                {
                    writer.WriteText(new ValidatedText("Test"));
                    return Task.CompletedTask;
                }, cancellationToken);
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head><title>Test</title></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithStringText()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                return writer.WriteElementAsync(new ValidatedElement("title"), null, (writer, cancellationToken) =>
                {
                    writer.WriteText("Test");
                    return Task.CompletedTask;
                }, cancellationToken);
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head><title>Test</title></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static async Task WriteDocumentWithReadOnlySpanCharText()
    {
        var buffer = new ArrayBufferWriter<byte>();
        await HtmlWriter.WriteDocumentAsync(buffer, null, (writer, cancellationToken) =>
        {
            return writer.WriteElementAsync(new ValidatedElement("head"), children: (writer, cancellationToken) =>
            {
                return writer.WriteElementAsync(new ValidatedElement("title"), null, (writer, cancellationToken) =>
                {
                    writer.WriteText("Test".AsSpan());
                    return Task.CompletedTask;
                }, cancellationToken);
            }, cancellationToken: cancellationToken);
        });

        Assert.Equal("<!DOCTYPE html><html><head><title>Test</title></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }
}
