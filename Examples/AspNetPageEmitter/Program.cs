using HtmlUtilities;
using HtmlUtilities.Validated;
using HtmlUtilities.Validated.Standardized;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var app = builder.Build();

app.Use((context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new()
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
    ValidatedAttributeValue IHtmlDocument.Language => new("en-us");
    ValidatedText IHtmlDocument.Title => new("Hello World!");
    ValidatedAttributeValue IHtmlDocument.Description => new("Test page for HTML Utilities");
    IReadOnlyCollection<Style> IHtmlDocument.Styles { get; } = [new() { Content = new(string.Join('\n', File.ReadAllLines("styles.css"))) }]; 

    Task IHtmlDocument.WriteBodyContentsAsync(HtmlWriter writer, CancellationToken cancellationToken)
    {
        writer.WriteElement("p", null, children => children.WriteText("Test bytes."));
        writer.WriteElement("p", null, children =>
        {
            children.WriteElement("a", attributes => attributes.Write("href", "/"), children => children.WriteText("Test link."));
        });
        writer.WriteScript(ValidatedScript.ForInlineSource("console.log('test')"));
        return Task.CompletedTask;
    }
}
