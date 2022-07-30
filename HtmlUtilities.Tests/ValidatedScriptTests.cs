namespace HtmlUtilities;

public static class ValidatedScriptTests
{
    [Theory]
    [InlineData("<! --")]
    [InlineData("< !--")]
    [InlineData("<!- -")]
    [InlineData("<scrip")]
    [InlineData("</scrip")]
    [InlineData("if(left < script)true;")]
    public static void InlineScriptWithNoAttributes(string script)
    {
        static void TestCase(string script)
        {
            Assert.Equal($"<script>{script}</script>", ValidatedScript.ForInlineSource(script).ToString());
        }

        TestCase(script);
        TestCase($"content{script}");
        TestCase($"content{script}content");
        TestCase($"{script}content");
        TestCase($"<>{script}");
        TestCase($"<>{script}<>");
        TestCase($"{script}<>");
        TestCase($"><{script}");
        TestCase($"><{script}><");
        TestCase($"{script}><");
    }

    [Theory]
    [InlineData("<!--")]
    [InlineData("<script")]
    [InlineData("</script")]
    [InlineData("<Script")]
    [InlineData("</Script")]
    [InlineData("<SCRIPT")]
    [InlineData("</SCRIPT")]
    public static void InlineScriptInvalidIsBlocked(string script)
    {
        static void TestCase(string script)
        {
            var x = Assert.Throws<ArgumentException>(() => ValidatedScript.ForInlineSource(script).ToString());
            Assert.Equal("script", x.ParamName);
        }

        TestCase(script);
        TestCase($"content{script}");
        TestCase($"content{script}content");
        TestCase($"{script}content");
        TestCase($"<>{script}");
        TestCase($"<>{script}<>");
        TestCase($"{script}<>");
        TestCase($"><{script}");
        TestCase($"><{script}><");
        TestCase($"{script}><");
    }
}
