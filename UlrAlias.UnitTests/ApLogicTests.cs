using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using UlrAlias.Backend.Dtos;
using UlrAlias.Backend.endpoints;
using UlrAlias.Backend.Models;
using UlrAlias.Backend.Services;

namespace UlrAlias.UnitTests;

public class ApLogicTests
{
    [Fact]
    public async Task HandleAliasRedirect_ReturnsFallback_WhenAliasNotFound()
    {
        var alias = "nonexistent";
        var context = new DefaultHttpContext();
        var mockService = new Mock<IAliasService>();
        mockService.Setup(s => s.TryGetAsync(alias, It.IsAny<CancellationToken>())).ReturnsAsync((AliasEntry?)null);

        var result = await ApLogic.HandleAliasRedirect(alias, context, mockService.Object);

        Assert.IsType<RedirectHttpResult>(result);

        if (result is RedirectHttpResult redirect)
            Assert.Contains("/swagger/index.html", redirect.Url);
    }

    [Fact]
    public async Task GetAlias_ReturnsOk_WhenAliasExists()
    {
        const string alias = "existing";
        var context = new DefaultHttpContext();
        var mockService = new Mock<IAliasService>();
        var aliasEntry = new AliasEntry(alias, "https://example.com", null);
        mockService.Setup(s => s.TryGetAsync(alias, It.IsAny<CancellationToken>())).ReturnsAsync(aliasEntry);

        var result = await ApLogic.GetAlias(alias, context, mockService.Object);

        Assert.IsType<Ok<AliasEntry>>(result);
    }

    [Fact]
    public async Task PostAlias_ReturnsCreated_WhenAliasAdded()
    {
        var input = new AliasEntryDto { Url = "https://example.com", Alias = "test" };
        var context = new DefaultHttpContext();
        var mockShortener = new Mock<IUrlShortener>();
        var mockService = new Mock<IAliasService>();
        mockService.Setup(s => s.AddAsync(It.IsAny<AliasEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AddResult.Added);

        var result = await ApLogic.PostAlias(input, context, mockShortener.Object, mockService.Object);

        Assert.IsType<Created<AliasEntryDto>>(result);
    }
}