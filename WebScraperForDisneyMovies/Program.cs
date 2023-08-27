using System;
using System.Collections.Generic;
using System.Diagnostics;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;

namespace WebScraperForDisneyMovies
{
    public class Program
    {
        static void Main(string[] args)
        {

            //service collection
            var services = new ServiceCollection();

            //Register the services
            services.AddSingleton<HtmlWeb>(); // Register HtmlWeb as a singleton
            services.AddTransient<Scraper>(); // Register Scraper as a transient service

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            //Resolve and use the Scraper instance
            using var scope = serviceProvider.CreateScope();
            var scraper = scope.ServiceProvider.GetRequiredService<Scraper>();

            Stopwatch sw = new();
            sw.Start();
            List<Movie> movies = new();
            const string site = "https://www.imdb.com",
            list = "/list/ls059383351/",
            xPath = "//*[@class='lister-item mode-detail']";
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
            if (db.disney_movies is null)
            {

                throw new NullReferenceException("disney_movies table is null; does not exist");
            }
            var FoundMovie = db.disney_movies
                .Find(67) ?? throw new NullReferenceException("wrong Id parameter or movie is not present in the database");
            Console.WriteLine(FoundMovie.title + "\t" + FoundMovie.year + "\n" + FoundMovie.image + "\n\t---------------------------");
            sw.Stop();
            string ExecutionTimeTaken = string.Format("{0} minutes, {1}seconds", sw.Elapsed.Minutes, sw.Elapsed.Seconds);

            Console.WriteLine("\nprogram finished in: " + ExecutionTimeTaken);

        }

    }
}
