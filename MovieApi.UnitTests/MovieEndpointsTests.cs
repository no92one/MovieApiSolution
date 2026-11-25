using Microsoft.AspNetCore.Http;
using MovieApi.DTOs;
using MovieApi.Endpoints;
using MovieApi.Models;
using MovieApi.Repositories;
using NSubstitute;
using Xunit;

namespace MovieApi.UnitTests;

public class MovieEndpointsTests
{
    [Fact]
    public async Task GetMovieAsync_ReturnsMovie()
    {
        // Arrange
        var repository = Substitute.For<IMovieRepository>();

        Movie movie = new(1, "Interstellar", "Christopher Nolan", 2014);

        repository.GetMovieAsync(1).Returns(movie);

        // Act
        var result = await repository.GetMovieAsync(1);
        
        // Assert
        Assert.Equal(movie, result);
    }
}