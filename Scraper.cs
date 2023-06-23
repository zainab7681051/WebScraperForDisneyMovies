using System.Text.RegularExpressions;

namespace WebScraperForDisneyMovies
{
    public static class Scraper
    {
        public static List<Movie> ScrapeData(string site, string list, string xPath)
        {
            List<Movie> movies = new();
            string url = site + list;
            string title = "", year = "", link = "",
                image = "", runtime = "", genre = "",
                summary = "", rating = "", metascore = "",
                directors = "", stars = "";

            var web = new HtmlAgilityPack.HtmlWeb();
            var doc = web.Load(url);
            var docNodes = doc.DocumentNode.SelectNodes(xPath);
            if (docNodes is null)
            {
                throw new NullReferenceException("docNodes is null");
            }

            int i = 0, nodeCollectionLength = docNodes.ToArray().Length;

            Console.WriteLine("Scraping the data...");
            foreach (var item in docNodes)
            {
                i++;
                // Update progress bar
                UpdateProgressBar(i, nodeCollectionLength);
                try
                {
                    if (item is null)
                    {
                        continue;
                    }
                    //LINK
                    link = item.Element("div").NextSibling.NextSibling
                    .Element("h3").ChildNodes["a"].Attributes["href"].Value.Trim();

                    //TITLE
                    title = item.Element("div").NextSibling.NextSibling
                    .Element("h3").ChildNodes["a"].InnerText.Trim();

                    // //YEAR
                    year = item.Element("div").NextSibling.NextSibling
                    .Element("h3").Elements("span").Where(i => i.HasClass("lister-item-year"))
                    .Single().InnerText.Trim();


                    // //GENRE AND RUNTIME
                    var detail = item.Element("div").NextSibling.NextSibling.Element("h3")
                    .NextSibling.NextSibling.ChildNodes;

                    genre = detail.Where(d => d.HasClass("genre")).Single().InnerText.Trim();

                    runtime = detail.Where(d => d.HasClass("runtime")).Single().InnerText.Trim();

                    // //RATING
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
                    var cast = item.Element("div")
                        .NextSibling.NextSibling.Element("div")
                        .NextSibling.NextSibling.NextSibling
                        .NextSibling.NextSibling.NextSibling.InnerText.Trim();
                    string directorsPattern = @"Director(?:s)?:\s*(.*?)(?:\s*\||$)";
                    string starsPattern = @"Stars:\s*(.*?)(?:\s*\||$)";

                    // Extract Directors
                    var directorsMatch = Regex.Match(cast, directorsPattern, RegexOptions.Singleline);
                    directors = directorsMatch.Groups[1].Value.Trim();

                    //  Extract Stars
                    var starsMatch = Regex.Match(cast, starsPattern, RegexOptions.Singleline);
                    stars = starsMatch.Groups[1].Value.Trim();

                    //IMAGE
                    var newTab = web.Load(site + link);
                    var metaTags = newTab.DocumentNode.SelectNodes("/html/head/meta");

                    foreach (var metaTag in metaTags)
                    {
                        string property = metaTag.GetAttributeValue("property", "not found");
                        if (property == "og:image")
                        {
                            image = metaTag.GetAttributeValue("content", "not found");
                        }
                    }
                }
                catch (Exception e)
                {
                    string stackT = e.StackTrace;
                    string eType = e.GetType().ToString();
                    if (eType == "System.NullReferenceException")
                    {

                        if (stackT.Contains("line 48"))
                            year = "";
                        if (stackT.Contains("line 56"))
                            genre = "";
                        if (stackT.Contains("line 58"))
                            runtime = "";
                        if (stackT.Contains("line 68"))
                            metascore = "";
                    }
                    else
                    {
                        Console.WriteLine(e);
                    }

                }
                finally
                {

                    movies.Add(new Movie
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
                    });

                }
            }
            Console.WriteLine("\nScraping Stage completed.\n");
            return movies;
        }


        private static void UpdateProgressBar(int currentIteration, int totalIterations)
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
