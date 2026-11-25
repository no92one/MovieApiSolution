namespace MovieApi.Models;

public record Movie(
    int Id,
    string Title,
    string Director,
    int ReleaseYear
);