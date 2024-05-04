using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebScraperForDisneyMovies
{
    public class Scraper
    {
        private readonly HtmlWeb _HtmlWeb;
        private List<HtmlDocument> Tabs;
        public Scraper()
        {
            _HtmlWeb = new HtmlWeb();
            Tabs=new();
        }
        public List<Movie> ScrapeData(string site, string list, string xPath)
        {
            List<Movie> movies = new();
            string url = site + list;
            var docNodes = _HtmlWeb.Load(url).DocumentNode.SelectNodes(xPath);
            if (docNodes is null)
            {
                throw new NullReferenceException("docNodes is null");
            }


            Console.WriteLine("\nthis program will take 2 to 3 minutes tops to finish\n");
            Console.WriteLine("\nanyway, scraping the data!");
            for (int i = 0; i < docNodes.ToArray().Length; i++)
            {
                UpdateProgressBar(i, docNodes.ToArray().Length - 1);
                Movie? movie = ScrapePage(docNodes[i], site);
                if (movie is null) continue;
                movies.Add(movie);
            }

            //IMAGE: get the high def image for each movie poster
            Console.WriteLine("\nadding high def images...");
            movies.TrimExcess();
            for(int i=0; i<movies.Count; i++)
            { 
                UpdateProgressBar(i, movies.Count-1);
                movies[i].image=GetImage(Tabs[i].DocumentNode.SelectNodes("/html/head/meta"));
            }

            Console.WriteLine("\nScraping Stage completed.\n");
            return movies;
        }

        private Movie? ScrapePage(HtmlNode item, string site)
        {
            string? title, year, link,
                runtime, genre,
                summary, rating, metascore,
                cast, directors, stars;

            if (item is null)
            {
                return null;
            }

            //TITLE
            title = GetTitle(item);

            //YEAR
            year = GetYear(item);

            // GENRE AND RUNTIME
            var detail = GetGenreAndRuntime(item);

            genre = detail?.Where(d => d.HasClass("genre")).Single()?.InnerText?.Trim() ?? "no genre";

            runtime = detail?.Where(d => d.HasClass("runtime")).Single()?.InnerText?.Trim() ?? "no runtime";

            // RATING
            rating = GetRating(item);

            // //METASCORE
            metascore = GetMetaScore(item);

            //SUMMARY
            summary = GetSummary(item);

            //CAST(DIRECTORS & STARS)
            cast = GetCast(item);

            // Extract Directors
            const string directorsPattern = @"Director(?:s)?:\s*(.*?)(?:\s*\||$)";
            const string starsPattern = @"Stars:\s*(.*?)(?:\s*\||$)";

            var directorsMatch = Regex.Match(cast, directorsPattern, RegexOptions.Singleline);
            directors = directorsMatch.Groups[1].Value.Trim();

            //  Extract Stars
            var starsMatch = Regex.Match(cast, starsPattern, RegexOptions.Singleline);
            stars = starsMatch.Groups[1].Value.Trim();

            //LINK
            link = GetLink(item);
            Tabs.Add(_HtmlWeb.Load(site + link));

            return new Movie
            {
                title = title,
                year = year,
                link = link,
                //image = image,
                runtime = runtime,
                genre = genre,
                summary = summary,
                rating = rating,
                metascore = metascore,
                directors = directors,
                stars = stars
            };
        }

        private string GetCast(HtmlNode item)
        {
            return item.Element("div")?
                .NextSibling?.NextSibling?.LastChild?.PreviousSibling?
                .PreviousSibling?.PreviousSibling?.PreviousSibling?
                .PreviousSibling.InnerText?.Trim() ?? "no cast";
        }

        private string GetSummary(HtmlNode item)
        {
            return item.Element("div")?
                .NextSibling?.NextSibling?.LastChild?.PreviousSibling?
                .PreviousSibling?.PreviousSibling?.PreviousSibling?
                .PreviousSibling?.PreviousSibling?.PreviousSibling.InnerText.Trim() ?? "no summary";
        }

        private string GetMetaScore(HtmlNode item)
        {
            return item.Element("div")?
            .NextSibling?.NextSibling?.Element("div")?
            .NextSibling?.NextSibling?.Element("span")?.InnerText?.Trim() ?? "no metascore";
        }

        private string GetRating(HtmlNode item)
        {
            return item.Element("div")?
                .NextSibling?.NextSibling?.Element("div")?
                .Element("div")?
                .ChildNodes?.Where(d => d.HasClass("ipl-rating-star__rating"))?
                .Single()?.InnerText?.Trim() ?? "no rating";
        }

        private HtmlNodeCollection? GetGenreAndRuntime(HtmlNode item)
        {
            return item.Element("div")?.NextSibling?.NextSibling?.Element("h3")?
            .NextSibling?.NextSibling?.ChildNodes;
        }

        private string GetYear(HtmlNode item)
        {
            return item.Element("div")?.NextSibling?.NextSibling?
            .Element("h3")?.Elements("span")?.Where(i => i.HasClass("lister-item-year"))
            .Single().InnerText?.Trim() ?? "no year";

        }

        private string GetTitle(HtmlNode item)
        {
            return item.Element("div")?.NextSibling?.NextSibling
            .Element("h3")?.ChildNodes["a"]?.InnerText?.Trim() ?? "no title";
        }

        private string GetLink(HtmlNode item)
        {
            return item.Element("div")?.NextSibling?.NextSibling?
            .Element("h3")?.ChildNodes["a"]?.Attributes["href"]?.Value?.Trim() ?? "no link";
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
            Console.CursorLeft = progressBarWidth + 5;
            Console.Write("  " + percentageString);
        }
    }
}
