using MovieApi.Models;
using MovieApi.DTOs;
using MySqlConnector;

namespace MovieApi.Repositories;

public class MovieRepository: IMovieRepository
{
    private readonly string _connectionString;

    public MovieRepository(IConfiguration configuration)
    {
        _connectionString = "Server=localhost;Port=3306;Database=movie_api;User ID=root;Password=kurslosenord;";
    }
    
    public async Task<List<Movie>> GetMoviesAsync()
    {
        List<Movie> movies = [];

        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "SELECT id, title, director, release_year FROM movies;",
            connection
        );

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Movie movie = new(
                reader.GetInt32("id"),
                reader.GetString("title"),
                reader.GetString("director"),
                reader.GetInt32("release_year")
            );

            movies.Add(movie);
        }

        return movies;
    }
    
    public async Task<Movie?> GetMovieAsync(int id)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            SELECT id, title, director, release_year
            FROM movies
            WHERE id = @id;
            """,
            connection
        );

        command.Parameters.AddWithValue("@id", id);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Movie(
                reader.GetInt32("id"),
                reader.GetString("title"),
                reader.GetString("director"),
                reader.GetInt32("release_year")
            );
        }

        return null;
    }
    
    public async Task<Movie> CreateMovieAsync(CreateMovieDto data)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            INSERT INTO movies (title, director, release_year)
            VALUES (@title, @director, @year);
            """,
            connection
        );

        command.Parameters.AddWithValue("@title", data.Title);
        command.Parameters.AddWithValue("@director", data.Director);
        command.Parameters.AddWithValue("@year", data.ReleaseYear);

        await command.ExecuteNonQueryAsync();

        int id = checked((int)command.LastInsertedId);

        return new Movie(
            id,
            data.Title,
            data.Director,
            data.ReleaseYear
        );
    }
    
    public async Task<bool> DeleteMovieAsync(int id)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "DELETE FROM movies WHERE id = @id;",
            connection
        );

        command.Parameters.AddWithValue("@id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }
    
    public async Task<Movie?> UpdateMovieAsync(
        int id,
        UpdateMovieDto data)
    {
        await using (var connection =
                     new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            await using var command = new MySqlCommand(
                """
                UPDATE movies
                SET title = @title,
                    director = @director,
                    release_year = @year
                WHERE id = @id;
                """,
                connection
            );

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@title", data.Title);
            command.Parameters.AddWithValue("@director", data.Director);
            command.Parameters.AddWithValue("@year", data.ReleaseYear);

            await command.ExecuteNonQueryAsync();
        }

        return await GetMovieAsync(id);
    }
    
    public async Task<Movie?> PatchMovieAsync(
        int id,
        PatchMovieDto data)
    {
        await using (var connection =
                     new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            await using var command = new MySqlCommand(
                """
                UPDATE movies
                SET title = COALESCE(@title, title),
                    director = COALESCE(@director, director),
                    release_year = COALESCE(@year, release_year)
                WHERE id = @id;
                """,
                connection
            );

            command.Parameters.AddWithValue("@id", id);

            command.Parameters.AddWithValue(
                "@title", (object?)data.Title ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@director", (object?)data.Director ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@year", (object?)data.ReleaseYear ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        return await GetMovieAsync(id);
    }
}