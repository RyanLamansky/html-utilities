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
            children.Write(new ValidatedElement("title"), null, children => children.Write(new ValidatedText("Hello World!")));
        }); //head

        await children.WriteAsync(new ValidatedElement("body"), null, async (children, cancellationToken) =>
        {
            children.Write(new ValidatedElement("p"), null, children => children.Write(new ValidatedText("First bytes.")));

            await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false);

            children.Write(new ValidatedElement("p"), null, children => children.Write(new ValidatedText("Second bytes after a delay.")));
        }).ConfigureAwait(false); // body
    }, cancellationToken).ConfigureAwait(false);

    await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
});

app.Run();