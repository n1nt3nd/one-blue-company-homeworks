using FluentAssertions;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class RateLimiterRepositoryTests
{
    private readonly IRateLimiterRepository _repository;

    public RateLimiterRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.RateLimiterRepository;
    }

    [Fact]
    async Task Set_SomeJson_Success()
    {
        // Arrange
        const string userIp = "192.168.0.1";
        const string expectedJson = "{ \"a\" : 3 }";

        await _repository.Set(userIp, expectedJson, TimeSpan.FromSeconds(1), default);

        // Act
        var actualJson = await _repository.Get(userIp, default);
        
        // Assert
        actualJson.Should().Be(expectedJson);
    }
}