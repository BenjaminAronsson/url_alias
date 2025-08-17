using UrlAlias.Services;

namespace UlrAlias.UnitTests;

public class UrlShortenerTests
{
    [Fact]
    public void GenerateAlias_ReturnsUniqueAlias()
    {
        var shortener = new UrlShortener();

        var alias1 = shortener.GenerateAlias("https://example.com");
        var alias2 = shortener.GenerateAlias("https://example.org");

        Assert.NotEqual(alias1, alias2);
    }

    [Fact]
    public void GenerateAlias_EncodesCorrectly()
    {
        var shortener = new UrlShortener();

        var alias = shortener.GenerateAlias("https://example.com");

        Assert.NotNull(alias);
        Assert.True(alias.Length > 0);
    }
}
