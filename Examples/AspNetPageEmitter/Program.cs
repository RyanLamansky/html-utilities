using HtmlUtilities;
using HtmlUtilities.Validated;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var app = builder.Build();

app.MapFallback((HttpContext context) =>
    context.WriteDocumentAsync(new TestDocument()));

await app.RunAsync().ConfigureAwait(false);

class TestDocument : IHtmlDocument
{
    ValidatedAttributeValue IHtmlDocument.Language => "en-us";
    ValidatedText IHtmlDocument.Title => "Hello World!";
    ValidatedAttributeValue IHtmlDocument.Description => "Test page for HTML Utilities";

    Task IHtmlDocument.WriteBodyContentsAsync(HtmlWriter writer, CancellationToken cancellationToken)
    {
        writer.WriteElement("p", null, children => children.WriteText("Test bytes."));
        return Task.CompletedTask;
    }
}

static class Extensions
{
    public static Task WriteDocumentAsync(this HttpContext context, IHtmlDocument document)
    {
        var request = context.Request;
        var response = context.Response;
        response.ContentType = "text/html; charset=utf-8";
        var baseUri = $"{request.Scheme}://{request.Host}/";
        response.Headers.ContentSecurityPolicy = $"default-src {baseUri}; base-uri {baseUri}";

        return document.WriteAsync(response.BodyWriter, context.RequestAborted);
    }
}