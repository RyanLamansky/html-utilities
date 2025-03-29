using HtmlUtilities;
using HtmlUtilities.Validated;
using HtmlUtilities.Validated.Standardized;

static Task<WebApplication> Startup(string[] args)
{
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

        context.Response.Headers.Append("Cross-Origin-Opener-Policy", "noopener-allow-popups");

        return next();
    });

    app.MapFallback(async context =>
    {
        // Since there's no async inside the home page in this example, we could use HtmlWriter.WriteDocument instead.
        await HtmlWriter.WriteDocumentAsync(context, new HomePage(), context.RequestAborted).ConfigureAwait(false);
    });

    return Task.FromResult(app); // This example doesn't have any async startup functions, but is ready to get them.
}

// This is the most direct way to run the web application.
// Standard "Main", no compiler-generated async state management, no extra stack frames.
Startup(args).GetAwaiter().GetResult().RunAsync().GetAwaiter().GetResult();

internal class HomePage : IHtmlDocument
{
    public void WriteHtmlAttributes(AttributeWriter writer)
    {
        writer.Write("lang", "en-US");
    }

    public void WriteHeadChildren(HtmlWriter children)
    {
        children.WriteElement("title", null, children => children.WriteText("Hello World!"));
        children.WriteElement("meta", attributes =>
        {
            attributes.Write("name", "viewport");
            attributes.Write("content", "width=device-width, initial-scale=1");
        });
        children.WriteElement("meta", attributes =>
        {
            attributes.Write("name", "description");
            attributes.Write("content", "Test page for HTML Utilities");
        });
        children.WriteElement(new Style
        {
            Content = new(string.Join('\n', File.ReadAllLines("styles.css"))),
        });
    }

    public void WriteBodyChildren(HtmlWriter children)
    {
        children.WriteElement("p", null, children => children.WriteText("Test bytes."));
        children.WriteElement("p", null, writer =>
        {
            writer.WriteElement(
                "a",
                attributes => attributes.Write("href", "/"),
                children => children.WriteText("Test link.")
                );
        });
        children.WriteScript(ValidatedScript.ForInlineSource("console.log('test')"));
    }
}