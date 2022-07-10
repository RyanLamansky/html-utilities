using HtmlUtilities;

var app = WebApplication.CreateBuilder(args).Build();

app.MapFallback(async (HttpResponse response, CancellationToken cancellationToken) =>
{
    response.ContentType = "text/html; charset=utf-8";

    await HtmlWriter.WriteDocumentAsync(response.BodyWriter, attributes => attributes.Write("lang", "en-us"), async children =>
    {
        children.WriteElement(new ValidatedElement("head"), null, children =>
        {
            children.WriteElementSelfClosing(new ValidatedElement("meta"), attributes => attributes.Write("charset", "utf-8"));
            children.WriteElement(new ValidatedElement("title"), null, children => children.WriteText(new ValidatedText("Hello World!")));
        }); //head

        await children.WriteElementAsync(new ValidatedElement("body"), null, async children =>
        {
            children.WriteElement(new ValidatedElement("p"), null, children => children.WriteText(new ValidatedText("First bytes.")));

            await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false);

            children.WriteElement(new ValidatedElement("p"), null, children => children.WriteText(new ValidatedText("Second bytes after a delay.")));
        }).ConfigureAwait(false); // body
    }).ConfigureAwait(false);

    await response.BodyWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
});

app.Run();