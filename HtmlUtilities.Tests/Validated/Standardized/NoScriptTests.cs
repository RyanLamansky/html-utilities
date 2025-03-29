namespace HtmlUtilities.Validated.Standardized;

public static class NoScriptTests
{
    [Fact]
    public static void NoScript() =>
        Assert.Equal("<noscript>test</noscript>"u8, new NoScript().ToUtf8(null, writer => writer.WriteText("test"u8)).Span);
}
