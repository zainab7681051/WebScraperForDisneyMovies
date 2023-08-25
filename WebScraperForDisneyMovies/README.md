# WebScraperForDisneyMovies
web scraper software in .NET for scraping IMDB Disney animation movies list page and store it in a Sqlite3 database in the root directory of this project using the Entity Framework package

```
dotnet tool install --global dotnet-ef 
```

```
dotnet restore
```

```
dotnet ef migrations add InitialCreate 
dotnet ef database update 
```

```
dotnet run
```

