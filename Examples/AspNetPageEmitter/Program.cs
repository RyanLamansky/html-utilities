using HtmlUtilities;

var app = WebApplication.CreateBuilder(args).Build();

app.MapFallback(async (HttpResponse response, CancellationToken cancellationToken) =>
{
    response.ContentType = "text/html; charset=utf-8";

    await HtmlWriter.WriteDocumentAsync(response.BodyWriter, attributes => attributes.Write("lang", "en-us"), async (children, cancellationToken) =>
    {
        children.WriteElement("head", null, children =>
        {
            children.WriteElementSelfClosing("meta", attributes => attributes.Write("charset", "utf-8"));
            children.WriteElement("title", null, children => children.WriteText("Hello World!"));
        }); //head

        await children.WriteElementAsync("body", null, async (children, cancellationToken) =>
        {
            children.WriteElement("p", null, children => children.WriteText("First bytes."));

            await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

            children.WriteElement("p", null, children => children.WriteText("Second bytes after a delay."));
        }, cancellationToken).ConfigureAwait(false); // body
    }, cancellationToken).ConfigureAwait(false);

    await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
});

app.Run();