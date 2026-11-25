namespace MovieApi.DTOs;

public record UpdateMovieDto(
    string Title,
    string Director,
    int ReleaseYear
);