using MovieApi.Models;
using MovieApi.DTOs;
using MovieApi.Repositories;

namespace MovieApi.Endpoints;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this WebApplication app)
    {
        var movies = app.MapGroup("/api/movies");

        movies.MapGet("", async (IMovieRepository repository) =>
        {
            List<Movie> result =
                await repository.GetMoviesAsync();

            return Results.Ok(result);
        });
        
        movies.MapGet("/{id}", GetMovieByIdAsync);
        
        movies.MapPost("", async (
            CreateMovieDto data,
            IMovieRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(data.Title) ||
                string.IsNullOrWhiteSpace(data.Director) ||
                data.ReleaseYear < 1888)
            {
                return Results.BadRequest(new
                {
                    message = "Invalid movie data."
                });
            }

            Movie movie = await repository.CreateMovieAsync(data);

            return Results.Created(
                $"/api/movies/{movie.Id}",
                movie
            );
        });
        
        movies.MapDelete("/{id}", async (
            string id,
            IMovieRepository repository) =>
        {
            if (!int.TryParse(id, out int movieId) || movieId <= 0)
            {
                return Results.BadRequest(new
                {
                    message = "Invalid movie ID."
                });
            }

            bool deleted = await repository.DeleteMovieAsync(movieId);

            if (!deleted)
            {
                return Results.NotFound(new
                {
                    message = "Movie not found."
                });
            }

            return Results.NoContent();
        });
        
        movies.MapPut("/{id}", async (
            int id,
            UpdateMovieDto data,
            IMovieRepository repository) =>
        {
            if (id <= 0 ||
                string.IsNullOrWhiteSpace(data.Title) ||
                string.IsNullOrWhiteSpace(data.Director) ||
                data.ReleaseYear < 1888)
            {
                return Results.BadRequest(new
                {
                    message = "Invalid movie data."
                });
            }

            Movie? movie = await repository.UpdateMovieAsync(id, data);

            if (movie is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(movie);
        });
        
        movies.MapPatch("/{id}", async (
            int id,
            PatchMovieDto data,
            IMovieRepository repository) =>
        {
            if (id <= 0 ||
                (data.Title is not null &&
                 string.IsNullOrWhiteSpace(data.Title)) ||
                (data.Director is not null &&
                 string.IsNullOrWhiteSpace(data.Director)) ||
                (data.ReleaseYear is not null &&
                 data.ReleaseYear < 1888))
            {
                return Results.BadRequest(new
                {
                    message = "Invalid movie data."
                });
            }

            Movie? movie = await repository.PatchMovieAsync(id, data);

            if (movie is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(movie);
        });
        
    }
    
    public static async Task<IResult> GetMovieByIdAsync(
        int id,
        IMovieRepository repository)
    {
        if (id <= 0)
        {
            return Results.BadRequest(new
            {
                message = "ID must be greater than zero."
            });
        }

        Movie? movie = await repository.GetMovieAsync(id);

        if (movie is null)
        {
            return Results.NotFound(new
            {
                message = "Movie not found."
            });
        }

        return Results.Ok(movie);
    }
}