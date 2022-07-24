using HtmlUtilities;

var app = WebApplication.CreateBuilder(args).Build();

app.MapFallback(async (HttpResponse response, CancellationToken cancellationToken) =>
{
    response.ContentType = "text/html; charset=utf-8";

    await HtmlWriter.WriteDocumentAsync(response.BodyWriter, attributes => attributes.Write("lang", "en-us"), async (children, cancellationToken) =>
    {
        children.Write(new ValidatedElement("head"), null, children =>
        {
            children.WriteSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
            children.Write(new ValidatedElement("title"), null, children => children.Write("Hello World!"));
        }); //head

        await children.WriteAsync(new ValidatedElement("body"), null, async (children, cancellationToken) =>
        {
            children.Write(new ValidatedElement("p"), null, children => children.Write("First bytes."));

            await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

            children.Write(new ValidatedElement("p"), null, children => children.Write("Second bytes after a delay."));
        }, cancellationToken).ConfigureAwait(false); // body
    }, cancellationToken).ConfigureAwait(false);

    await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
});

app.Run();