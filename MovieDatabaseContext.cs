using Microsoft.EntityFrameworkCore;
namespace WebScraperForDisneyMovies
{
    public class MovieDatabaseContext : DbContext
    {
        public DbSet<Movie>? disney_movies { get; set; }
        public string DbPath { get; }

        public MovieDatabaseContext()
        {
            DbPath = "./DisneyMoviesDB.db";
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    }
}