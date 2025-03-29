namespace HtmlUtilities.Validated.Standardized;

public static class ImportMapTests
{
    private static void AssertEqual(string expected, ImportMap map)
    {
        Assert.Equal(expected, map.ToString());
    }

    [Fact]
    public static void Empty() => AssertEqual("{}", new ImportMap());

    [Fact]
    public static void Populated() => AssertEqual(
#if DEBUG
        """
        {
          "imports": {
            "a": "b"
          },
          "integrity": {
            "c": "d"
          },
          "scopes": {
            "e": {
              "f": "g"
            }
          }
        }
        """,
        
#else
        """
        {"imports":{"a":"b"},"integrity":{"c":"d"},"scopes":{"e":{"f":"g"}}}
        """,
#endif
        new ImportMap
        {
            Imports = new Dictionary<string, string>
            {
                { "a","b" },
            },
            Integrity = new Dictionary<string, string>
            {
                { "c","d" },
            },
            Scopes = new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                { "e", new Dictionary<string, string> { { "f", "g" } } }
            },
        });
}
