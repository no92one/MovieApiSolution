using MovieApi.Models;
using MySqlConnector;

namespace MovieApi.Repositories;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = "Server=localhost;Port=3306;Database=movie_api;User ID=root;Password=kurslosenord;";
    }
    
    public async Task<User> CreateUserAsync(
        string username,
        string passwordHash)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            INSERT INTO users (username, password_hash)
            VALUES (@username, @passwordHash);
            """,
            connection
        );

        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@passwordHash", passwordHash);

        await command.ExecuteNonQueryAsync();

        int id = checked((int)command.LastInsertedId);

        return new User(id, username, passwordHash);
    }
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            SELECT id, username, password_hash
            FROM users
            WHERE username = @username;
            """,
            connection
        );

        command.Parameters.AddWithValue("@username", username);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User(
                reader.GetInt32("id"),
                reader.GetString("username"),
                reader.GetString("password_hash")
            );
        }

        return null;
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            SELECT id, username, password_hash
            FROM users
            WHERE id = @id;
            """,
            connection
        );

        command.Parameters.AddWithValue("@id", id);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User(
                reader.GetInt32("id"),
                reader.GetString("username"),
                reader.GetString("password_hash")
            );
        }

        return null;
    }
}