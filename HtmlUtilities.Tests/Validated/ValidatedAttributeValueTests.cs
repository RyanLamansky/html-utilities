namespace HtmlUtilities.Validated;

public static class ValidatedAttributeValueTests
{
    [Fact]
    public static void ValidatedAttributeValueUnconstructedIsBlank()
    {
        var array = new ValidatedAttributeValue[1];

        Assert.Empty(array[0].ToString());
    }

    [Theory]
    [InlineData("en", "=en")]
    [InlineData("en-us", "=en-us")]
    [InlineData("top left", "=\"top left\"")]
    [InlineData("d&d", "=d&amp;d")]
    [InlineData("Dungeons & Dragons", "=\"Dungeons &amp; Dragons\"")]
    public static void AttributeValueIsFormattedCorrectly(string source, string expected)
    {
        Assert.Equal(expected, new ValidatedAttributeValue(source).ToString());
    }

    [Fact]
    public static void AttributeValueFromNullIsEmpty()
    {
        Assert.Equal("=\"\"", new ValidatedAttributeValue((string?)null).ToString());
        Assert.Equal("=\"\"", new ValidatedAttributeValue((int?)null).ToString());
        Assert.Equal("=\"\"", new ValidatedAttributeValue((uint?)null).ToString());
        Assert.Equal("=\"\"", new ValidatedAttributeValue((long?)null).ToString());
        Assert.Equal("=\"\"", new ValidatedAttributeValue((ulong?)null).ToString());
    }

    [Theory]
    [InlineData("=5", 5)]
    [InlineData("=13", 13)]
    [InlineData("=134", 134)]
    [InlineData("=-5", -5)]
    [InlineData("=-13", -13)]
    [InlineData("=-134", -134)]
    [InlineData("=2147483647", int.MaxValue)]
    [InlineData("=-2147483648", int.MinValue)]
    public static void AttributeValueFromInt32(string expected, int number)
    {
        Assert.Equal(expected, new ValidatedAttributeValue(number).ToString());
        Assert.Equal(expected, new ValidatedAttributeValue((int?)number).ToString());
    }

    [Theory]
    [InlineData("=5", 5)]
    [InlineData("=13", 13)]
    [InlineData("=134", 134)]
    [InlineData("=2147483647", int.MaxValue)]
    [InlineData("=4294967295", uint.MaxValue)]
    public static void AttributeValueFromUInt32(string expected, uint number)
    {
        Assert.Equal(expected, new ValidatedAttributeValue(number).ToString());
        Assert.Equal(expected, new ValidatedAttributeValue((uint?)number).ToString());
    }

    [Theory]
    [InlineData("=5", 5)]
    [InlineData("=13", 13)]
    [InlineData("=134", 134)]
    [InlineData("=-5", -5)]
    [InlineData("=-13", -13)]
    [InlineData("=-134", -134)]
    [InlineData("=2147483647", int.MaxValue)]
    [InlineData("=-2147483648", int.MinValue)]
    [InlineData("=9223372036854775807", long.MaxValue)]
    [InlineData("=-9223372036854775808", long.MinValue)]
    public static void AttributeValueFromInt64(string expected, long number)
    {
        Assert.Equal(expected, new ValidatedAttributeValue(number).ToString());
        Assert.Equal(expected, new ValidatedAttributeValue((long?)number).ToString());
    }

    [Theory]
    [InlineData("=5", 5)]
    [InlineData("=13", 13)]
    [InlineData("=134", 134)]
    [InlineData("=2147483647", int.MaxValue)]
    [InlineData("=4294967295", uint.MaxValue)]
    [InlineData("=18446744073709551615", ulong.MaxValue)]
    public static void AttributeValueFromUInt64(string expected, ulong number)
    {
        Assert.Equal(expected, new ValidatedAttributeValue(number).ToString());
        Assert.Equal(expected, new ValidatedAttributeValue((ulong?)number).ToString());
    }
}