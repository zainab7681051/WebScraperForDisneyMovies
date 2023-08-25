using HtmlAgilityPack;
using Moq;
using Xunit;

public class ScraperTest
{
    // [Fact]
    // public void GetImage_ReturnsValidImageURL()
    // {
    //     // Arrange
    //     var htmlWebMock = new Mock<HtmlWeb>();
    //     var htmlDocument = new HtmlDocument();
    //     htmlDocument.LoadHtml(@"
    //         <html>
    //             <head>
    //                 <meta property='og:image' content='https://example.com/image.jpg' />
    //             </head>
    //             <body></body>
    //         </html>");
    //     var htmlWebLoadResult = new HtmlWebLoadResult(htmlDocument);

    //     htmlWebMock.Setup(web => web.Load(It.IsAny<string>())).Returns(htmlWebLoadResult);

    //     var extractor = new ImageExtractor(htmlWebMock.Object);
    //     var metaTags = htmlDocument.DocumentNode.SelectNodes("/html/head/meta");

    //     // Act
    //     var imageUrl = extractor.GetImage(metaTags);

    //     // Assert
    //     Assert.Equal("https://example.com/image.jpg", imageUrl);
    // }

    // [Fact]
    // public void GetImage_ReturnsNoImageIfNotFound()
    // {
    //     // Arrange
    //     var htmlWebMock = new Mock<HtmlWeb>();
    //     var htmlDocument = new HtmlDocument();
    //     htmlDocument.LoadHtml("<html><head></head><body></body></html>");
    //     var htmlWebLoadResult = new HtmlWebLoadResult(htmlDocument);

    //     htmlWebMock.Setup(web => web.Load(It.IsAny<string>())).Returns(htmlWebLoadResult);

    //     var extractor = new ImageExtractor(htmlWebMock.Object);
    //     var metaTags = htmlDocument.DocumentNode.SelectNodes("/html/head/meta");

    //     // Act
    //     var imageUrl = extractor.GetImage(metaTags);

    //     // Assert
    //     Assert.Equal("no image", imageUrl);
    // }
}
