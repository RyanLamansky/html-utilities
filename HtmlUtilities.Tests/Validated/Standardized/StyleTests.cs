using System.Buffers;
using System.Text;

namespace HtmlUtilities.Validated.Standardized;

public static class StyleTests
{
    [Fact]
    public static void StyleMedia()
    {
        Assert.Equal("<style media=test></style>", new Style { Media = "test" }.ToString());
    }
}
