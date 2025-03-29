using HtmlUtilities.Validated.Standardized;

namespace HtmlUtilities.Validated;

// StandardElement isn't directly testable since it's abstract with an internal constructor.
// To test its features, one of its implementations must be used.
public static class StandardElementTests
{
    [Fact]
    public static void AttributeCanBeCleared()
    {
        var element = new LineBreak { Id = new("test"u8) };
        element.Id = null;
        Assert.Equal("<br>"u8, element.ToUtf8().Span);
    }

    [Fact]
    public static void BaseAttributes()
    {
        var element = new LineBreak
        {
            AccessKey = new("a"u8),
            Id = new("b"u8),
            Title = new("c"u8),
        };

        Assert.Equal("<br accesskey=a id=b title=c>"u8, element.ToUtf8().Span);
    }
}
