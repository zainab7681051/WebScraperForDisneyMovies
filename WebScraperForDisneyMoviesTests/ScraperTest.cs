using HtmlAgilityPack;
using Moq;
using WebScraperForDisneyMovies;
using Xunit;
namespace WebScraperForDisneyMoviesTests;
public class ScraperTest
{
    private readonly Scraper _Scraper;
    private readonly Mock<HtmlWeb> _HtmlWebMock = new();
    public ScraperTest()
    {
        _Scraper = new Scraper(_HtmlWebMock.Object);
    }
    [Fact]
    public void GetImage_ReturnsValidImageUrl()
    {
    }
}
