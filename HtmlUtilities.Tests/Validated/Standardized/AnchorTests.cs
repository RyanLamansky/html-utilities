namespace HtmlUtilities.Validated.Standardized;

public static class AnchorTests
{
    [Fact]
    public static void AnchorPopulated() =>
        Assert.Equal("<a href=a>test</a>"u8, new Anchor
        {
            Href = new("a"u8),
        }.ToUtf8(null, writer => writer.WriteText("test"u8)).Span);

    [Fact]
    public static void AnchorDynamicHref() =>
        Assert.Equal("<a href=test>test</a>"u8, new Anchor
        {
        }.ToUtf8(attributes => attributes.Write("href"u8, "test"u8), writer => writer.WriteText("test"u8)).Span);

    [Fact]
    public static void AnchorNativeAndDynamicHref() =>
        Assert.Equal("<a href=native href=dynamic>test</a>"u8, new Anchor
        {
            Href = new("native"u8),
        }.ToUtf8(attributes => attributes.Write("href"u8, "dynamic"u8), writer => writer.WriteText("test"u8)).Span);
}
