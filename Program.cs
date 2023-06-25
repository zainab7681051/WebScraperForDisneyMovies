using System.Diagnostics;

namespace WebScraperForDisneyMovies
{
    public class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new();
            sw.Start();
            List<Movie> movies = new();
            string site = "https://www.imdb.com",
            list = "/list/ls059383351/",
            xPath = "//*[@class='lister-item mode-detail']";
            var scraper = new Scraper();
            movies = scraper.ScrapeData(site, list, xPath);

            using var db = new MovieDatabaseContext();

            //Write            
            foreach (var movie in movies)
            {
                Console.WriteLine($"adding \"{movie.title}\" to the database...");
                db.Add(movie);
                db.SaveChanges();
            }

            // Read
            Console.WriteLine("\nQuerying for a Movie");
            var FoundMovie = db.disney_movies
                .Find(19);
            if (FoundMovie is null)
            {
                throw new NullReferenceException("wrong Id parameter or movie is not present in the database");

            }
            Console.WriteLine(FoundMovie.title + "\t" + FoundMovie.year + "\n" + FoundMovie.directors + "\n\t---------------------------");
            sw.Stop();
            string ExecutionTimeTaken = string.Format("{0} minutes, {1}seconds", sw.Elapsed.Minutes, sw.Elapsed.Seconds);

            Console.WriteLine("\nprogram finished in: " + ExecutionTimeTaken);


        }

    }
}
