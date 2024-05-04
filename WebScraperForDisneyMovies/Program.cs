#define DATABASE

using System.Diagnostics;

namespace WebScraperForDisneyMovies
{
    public class Program
    {
        static void Main(string[] args)
        {
           Scraper scraper =new (); 
            Stopwatch sw = new ();
            sw.Start();
            List<Movie> movies = new();
            const string site = "https://www.imdb.com",
            list = "/list/ls059383351/",
            xPath = "//*[@class='lister-item mode-detail']";
            movies = scraper.ScrapeData(site, list, xPath);

#if !DATABASE
#warning proceeding without database conection
#endif

#if DATABASE
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
            if (db.disney_movies is null)
            {

                throw new NullReferenceException("disney_movies table is null; does not exist");
            }
            var FoundMovie = db.disney_movies
                .Find(67) ?? throw new NullReferenceException("wrong Id parameter or movie is not present in the database");

            Console.WriteLine(FoundMovie.title + "\t" + FoundMovie.year + "\n" + FoundMovie.image + "\n\t---------------------------");

#endif

            sw.Stop();
            string ExecutionTimeTaken = string.Format("{0} minutes, {1}seconds", sw.Elapsed.Minutes, sw.Elapsed.Seconds);

            Console.WriteLine("\nprogram finished in: " + ExecutionTimeTaken);

        }

    }
}
