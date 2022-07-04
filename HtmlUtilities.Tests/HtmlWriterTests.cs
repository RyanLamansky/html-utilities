using System.Buffers;
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
        HtmlWriter.WriteDocument(buffer, children: writer => writer.WriteElement("head"));

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
        HtmlWriter.WriteDocument(buffer, attributes => attributes.Write("lang", "en-us"), writer => writer.WriteElement("head"));

        Assert.Equal("<!DOCTYPE html><html lang=en-us><head></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public static void WriteDocumentWithSelfClosing()
    {
        var buffer = new ArrayBufferWriter<byte>();
        HtmlWriter.WriteDocument(buffer, null, writer =>
        {
            writer.WriteElement("head", children: writer =>
            {
                writer.WriteElementSelfClosing("meta", attributes => attributes.Write("charset", "utf-8"));
            });
        });

        Assert.Equal("<!DOCTYPE html><html><head><meta charset=utf-8></head></html>", Encoding.UTF8.GetString(buffer.WrittenSpan));
    }
}
