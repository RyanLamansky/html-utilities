using HtmlUtilities;
using HtmlUtilities.Validated;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var app = builder.Build();

app.Use((context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
    {
        Private = true,
        NoCache = true,
    };

    return next();
});
app.MapFallback(new TestDocument().WriteToAsync);

await app.RunAsync().ConfigureAwait(false);

class TestDocument : IHtmlDocument
{
    ValidatedAttributeValue IHtmlDocument.Language => "en-us";
    ValidatedText IHtmlDocument.Title => "Hello World!";
    ValidatedAttributeValue IHtmlDocument.Description => "Test page for HTML Utilities";

    Task IHtmlDocument.WriteBodyContentsAsync(HtmlWriter writer, CancellationToken cancellationToken)
    {
        writer.WriteElement("p", null, children => children.WriteText("Test bytes."));
        writer.WriteScript(ValidatedScript.ForInlineSource("console.log('test')"));
        return Task.CompletedTask;
    }
}
