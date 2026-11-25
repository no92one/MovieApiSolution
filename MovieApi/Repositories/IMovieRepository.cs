using MovieApi.DTOs;
using MovieApi.Models;

namespace MovieApi.Repositories;

public interface IMovieRepository
{
    Task<List<Movie>> GetMoviesAsync();
    Task<Movie?> GetMovieAsync(int id);
    Task<Movie> CreateMovieAsync(CreateMovieDto data);
    Task<bool> DeleteMovieAsync(int id);
    Task<Movie?> UpdateMovieAsync(int id, UpdateMovieDto data);
    Task<Movie?> PatchMovieAsync(int id, PatchMovieDto data);
}