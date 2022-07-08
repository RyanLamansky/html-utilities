﻿using System.Buffers;
using System.Text;

namespace HtmlUtilities;

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
    public static void WriteChildElement()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, children: writer => writer.WriteElement(new ValidatedElement("head")));

        Assert.Equal("<!DOCTYPE html><html><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
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
    public static void WriteDocumentWithSelfClosing()
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
    public static void WriteDocumentWithSelfClosingInBody()
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
}
