using UrlAlias;

namespace UlrAlias.UnitTests;

public class UrlValidatorTests
{
    [Fact]
    public void IsValid_ReturnsFalse_WhenUrlIsEmpty()
    {
        var result = UrlValidator.IsValid("");

        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenUrlIsInvalid()
    {
        var result = UrlValidator.IsValid("invalid-url");

        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenUrlIsValid()
    {
        var result = UrlValidator.IsValid("https://example.com");

        Assert.True(result);
    }
}