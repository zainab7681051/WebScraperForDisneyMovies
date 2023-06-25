using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebScraperForDisneyMovies
{
    public class Scraper
    {
        public async Task<List<Movie>> ScrapeData(string site, string list, string xPath)
        {
            List<Movie> movies = new();
            string url = site + list;
            var web = new HtmlAgilityPack.HtmlWeb();
            var doc = await Task.Run(() => web.Load(url));
            var docNodes = await Task.Run(() => doc.DocumentNode.SelectNodes(xPath));
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
                Movie movie = await Task.Run(() => ScrapePage(docNodes[i], site));
                if (movie is null) continue;
                movies.Add(movie);
            }
            Console.WriteLine("\nScraping Stage completed.\n");
            return movies;
        }

        private async Task<Movie> ScrapePage(HtmlNode item, string site)
        {
            string title = "", year = "", link = "",
                image = "", runtime = "", genre = "",
                summary = "", rating = "", metascore = "",
                cast = "", directors = "", stars = "",
                directorsPattern = @"Director(?:s)?:\s*(.*?)(?:\s*\||$)",
                starsPattern = @"Stars:\s*(.*?)(?:\s*\||$)";
            try
            {
                if (item is null)
                {
                    return null;
                }
                //LINK
                link = item.Element("div").NextSibling.NextSibling
                .Element("h3").ChildNodes["a"].Attributes["href"].Value.Trim();

                //TITLE
                title = item.Element("div").NextSibling.NextSibling
                .Element("h3").ChildNodes["a"].InnerText.Trim();

                //YEAR
                year = item.Element("div").NextSibling.NextSibling
                .Element("h3").Elements("span").Where(i => i.HasClass("lister-item-year"))
                .Single().InnerText.Trim();


                // GENRE AND RUNTIME
                var detail = item.Element("div").NextSibling.NextSibling.Element("h3")
                .NextSibling.NextSibling.ChildNodes;

                genre = detail.Where(d => d.HasClass("genre")).Single().InnerText.Trim();

                runtime = detail.Where(d => d.HasClass("runtime")).Single().InnerText.Trim();

                // RATING
                rating = item.Element("div")
                    .NextSibling.NextSibling.Element("div")
                    .Element("div")
                    .ChildNodes.Where(d => d.HasClass("ipl-rating-star__rating"))
                    .Single().InnerText.Trim();

                // //METASCORE
                metascore = item.Element("div")
                    .NextSibling.NextSibling.Element("div")
                    .NextSibling.NextSibling.Element("span").InnerText.Trim();
                // //SUMMARY
                summary = item.Element("div")
                    .NextSibling.NextSibling.Element("div")
                    .NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Trim();

                // //CAST(DIRECTORS & STARS)
                cast = item.Element("div")
                    .NextSibling.NextSibling.Element("div")
                    .NextSibling.NextSibling.NextSibling
                    .NextSibling.NextSibling.NextSibling.InnerText.Trim();

                // Extract Directors
                var directorsMatch = Regex.Match(cast, directorsPattern, RegexOptions.Singleline);
                directors = directorsMatch.Groups[1].Value.Trim();

                //  Extract Stars
                var starsMatch = Regex.Match(cast, starsPattern, RegexOptions.Singleline);
                stars = starsMatch.Groups[1].Value.Trim();

                // //IMAGE
                var web = new HtmlAgilityPack.HtmlWeb();
                var newTab = await Task.Run(() => web.Load(site + link));
                var metaTags = await Task.Run(() => newTab.DocumentNode.SelectNodes("/html/head/meta"));
                image = await Task.Run(string () =>
                {
                    foreach (var metaTag in metaTags)
                    {
                        string property = metaTag.GetAttributeValue("property", "not found");
                        if (property == "og:image")
                        {
                            return metaTag.GetAttributeValue("content", "not found");
                        }
                    }
                    return null;
                });
                if (image is null) image = " ";

            }
            catch (Exception e)
            {
                string stackT = e.StackTrace;
                string eType = e.GetType().ToString();
                if (eType == "System.NullReferenceException")
                {

                    if (stackT.Contains("line 58"))
                        year = " ";
                    if (stackT.Contains("line 67"))
                        genre = " ";
                    if (stackT.Contains("line 69"))
                        runtime = " ";
                    if (stackT.Contains("line 79"))
                        metascore = " ";
                }
                else
                {
                    Console.WriteLine(e);
                }

            }

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
