using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebScraperForDisneyMovies
{
    public class Scraper
    {
        private readonly HtmlWeb _HtmlWeb;
        public Scraper(HtmlWeb HtmlWeb)
        {
            _HtmlWeb = HtmlWeb;
        }
        public List<Movie> ScrapeData(string site, string list, string xPath)
        {
            List<Movie> movies = new();
            string url = site + list;
            var doc = _HtmlWeb.Load(url);
            var docNodes = doc.DocumentNode.SelectNodes(xPath);
            if (docNodes is null)
            {
                throw new NullReferenceException("docNodes is null");
            }

            int nodeCollectionLength = docNodes.ToArray().Length;
            List<Movie> movieList = new();

            Console.WriteLine("\nScraping the data....");
            for (int i = 0; i < nodeCollectionLength; i++)
            {
                UpdateProgressBar(i, nodeCollectionLength - 1);
                Movie? movie = ScrapePage(docNodes[i], site);
                if (movie is null) continue;
                movies.Add(movie);
            }
            Console.WriteLine("\nScraping Stage completed.\n");
            return movies;
        }

        private Movie? ScrapePage(HtmlNode item, string site)
        {
            string? title, year, link,
                image, runtime, genre,
                summary, rating, metascore,
                cast, directors, stars;

            string directorsPattern = @"Director(?:s)?:\s*(.*?)(?:\s*\||$)",
            starsPattern = @"Stars:\s*(.*?)(?:\s*\||$)";
            if (item is null)
            {
                return null;
            }
            //LINK
            link = item.Element("div")?.NextSibling?.NextSibling?
            .Element("h3")?.ChildNodes["a"]?.Attributes["href"]?.Value?.Trim() ?? "no link";

            //TITLE
            title = item.Element("div")?.NextSibling?.NextSibling
            .Element("h3")?.ChildNodes["a"]?.InnerText?.Trim() ?? "no title";

            //YEAR
            year = item.Element("div")?.NextSibling?.NextSibling?
            .Element("h3")?.Elements("span")?.Where(i => i.HasClass("lister-item-year"))
            .Single().InnerText?.Trim() ?? "no year";


            // GENRE AND RUNTIME
            var detail = item.Element("div")?.NextSibling?.NextSibling?.Element("h3")?
            .NextSibling?.NextSibling?.ChildNodes;

            genre = detail?.Where(d => d.HasClass("genre")).Single()?.InnerText?.Trim() ?? "no genre";

            runtime = detail?.Where(d => d.HasClass("runtime")).Single()?.InnerText?.Trim() ?? "no runtime";

            // RATING
            rating = item.Element("div")?
                .NextSibling?.NextSibling?.Element("div")?
                .Element("div")?
                .ChildNodes?.Where(d => d.HasClass("ipl-rating-star__rating"))?
                .Single()?.InnerText?.Trim() ?? "no rating";

            // //METASCORE
            metascore = item.Element("div")?
            .NextSibling?.NextSibling?.Element("div")?
            .NextSibling?.NextSibling?.Element("span")?.InnerText?.Trim() ?? "no metascore";

            //SUMMARY
            // summary = item.Element("div")?
            //     .NextSibling?.NextSibling?.Element("div")?
            //     .NextSibling?.NextSibling?.NextSibling?.NextSibling?.InnerText?.Trim() ?? "no summary";
            summary = item.Element("div")?
                .NextSibling?.NextSibling?.LastChild?.PreviousSibling?
                .PreviousSibling?.PreviousSibling?.PreviousSibling?
                .PreviousSibling?.PreviousSibling?.PreviousSibling.InnerText.Trim() ?? "no summary";

            //CAST(DIRECTORS & STARS)
            cast = item.Element("div")?
                .NextSibling?.NextSibling?.LastChild?.PreviousSibling?
                .PreviousSibling?.PreviousSibling?.PreviousSibling?
                .PreviousSibling.InnerText?.Trim() ?? "no cast";

            // Extract Directors
            var directorsMatch = Regex.Match(cast, directorsPattern, RegexOptions.Singleline);
            directors = directorsMatch.Groups[1].Value.Trim();

            //  Extract Stars
            var starsMatch = Regex.Match(cast, starsPattern, RegexOptions.Singleline);
            stars = starsMatch.Groups[1].Value.Trim();

            //IMAGE
            var newTab = _HtmlWeb.Load(site + link);
            var metaTags = newTab.DocumentNode.SelectNodes("/html/head/meta");
            image = GetImage(metaTags);

            return new Movie
            {
                title = title,
                year = year,
                link = link,
                image = image,
                runtime = runtime,
                genre = genre,
                summary = summary,
                rating = rating,
                metascore = metascore,
                directors = directors,
                stars = stars
            };
        }

        private string GetImage(HtmlNodeCollection metaTags)
        {
            foreach (var metaTag in metaTags)
            {
                string property = metaTag.GetAttributeValue("property", "not found");
                if (property == "og:image")
                {
                    return metaTag.GetAttributeValue("content", "not found");
                }
            }
            return "no image";
        }
        private void UpdateProgressBar(int currentIteration, int totalIterations)
        {
            const int progressBarWidth = 50;

            double progressPercentage = (double)currentIteration / totalIterations;

            int filledSlots = (int)(progressPercentage * progressBarWidth);
            int emptySlots = progressBarWidth - filledSlots;

            string progressBar = "[" + new string('#', filledSlots) + new string('-', emptySlots) + "]";

            int percentage = (int)(progressPercentage * 100);
            string percentageString = percentage.ToString("D2") + "%";

            Console.CursorLeft = 0;
            Console.Write(progressBar);
            Console.CursorLeft = progressBarWidth + 1;
            Console.Write("  " + percentageString);
        }
    }
}
