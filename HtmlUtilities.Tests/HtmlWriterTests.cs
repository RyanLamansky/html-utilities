using System.Buffers;
using System.Text;

namespace HtmlUtilities;

using Validated;

public static class HtmlWriterTests
{
    [Fact]
    public static void EmptyDocument()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer);

        Assert.Equal("<!DOCTYPE html><html></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteChildElementValidated()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, children: writer => writer.WriteElement(new ValidatedElement("head")));

        Assert.Equal("<!DOCTYPE html><html><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteChildElementString()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, children: writer => writer.WriteElement("head"));

        Assert.Equal("<!DOCTYPE html><html><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteChildElementReadOnlySpan()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, children: writer => writer.WriteElement("head".AsSpan()));

        Assert.Equal("<!DOCTYPE html><html><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteUnconstructedChildElementThrows()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var array = new ValidatedElement[1];

        Assert.Equal("element", Assert.Throws<ArgumentException>(() => HtmlWriter.WriteDocument(buffer, children: writer => writer.WriteElement(array[0]))).ParamName);
    }

    [Fact]
    public static void WriteAttribute()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, attributes => attributes.Write("lang", "en-us"));

        Assert.Equal("<!DOCTYPE html><html lang=en-us></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteAttributeAndChildElement()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, attributes => attributes.Write("lang", "en-us"), writer => writer.WriteElement(new ValidatedElement("head")));

        Assert.Equal("<!DOCTYPE html><html lang=en-us><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingValidatedHead()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("head"), children: writer =>
            {
                writer.WriteElementSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingStringHead()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement("head", children: writer =>
            {
                writer.WriteElementSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingReadOnlySpanHead()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement("head".AsSpan(), children: writer =>
            {
                writer.WriteElementSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingValidated()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("head"), children: writer =>
            {
                writer.WriteElementSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingString()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("head"), children: writer =>
            {
                writer.WriteElementSelfClosing("meta", attributes => attributes.Write("charset", "utf-8"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingReadOnlySpan()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("head"), children: writer =>
            {
                writer.WriteElementSelfClosing("meta".AsSpan(), attributes => attributes.Write("charset", "utf-8"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingValidatedInBody()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("body"), children: writer =>
            {
                writer.WriteElementSelfClosing(new ValidatedElement("p"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><body><p></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingStringInBody()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("body"), children: writer =>
            {
                writer.WriteElementSelfClosing("p");
            });
        });

        Assert.Equal("<!DOCTYPE html><html><body><p></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosingReadOnlySpanCharInBody()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("body"), children: writer =>
            {
                writer.WriteElementSelfClosing("p".AsSpan());
            });
        });

        Assert.Equal("<!DOCTYPE html><html><body><p></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithMixedAttributes()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("body"), children: writer =>
            {
                writer.WriteElement(new ValidatedElement("div", new ValidatedAttribute[] { new("id", "react-app")}), attributes => attributes.Write("class", "root"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><body><div id=react-app class=root></div></body></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithValidatedText()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("head"), children: writer =>
            {
                writer.WriteElement(new ValidatedElement("title"), null, children => children.WriteText(new ValidatedText("Test")));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><title>Test</title></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithStringText()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("head"), children: writer =>
            {
                writer.WriteElement(new ValidatedElement("title"), null, children => children.WriteText("Test"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><title>Test</title></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithReadOnlySpanCharText()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement(new ValidatedElement("head"), children: writer =>
            {
                writer.WriteElement(new ValidatedElement("title"), null, children => children.WriteText("Test".AsSpan()));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><title>Test</title></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }
}
