using System.Text.Json;
using FluentAssertions;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Utils.Helpers;
using Moq;
using Xunit;

namespace HomeworkApp.UnitTests;

public class RateLimiterTests
{
    private readonly IRateLimiterService _rateLimiterService;
    private readonly Mock<IRateLimiterRepository> _rateLimiterRepositoryMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

    public RateLimiterTests()
    {
        _rateLimiterService = new RateLimiterService(_rateLimiterRepositoryMock.Object, _dateTimeProviderMock.Object);
    }

    [Fact]
    public async Task Allow_LastRequestLeft_Allowed()
    {
        // Arrange
        const string userIp = "192.168.0.0";
        var date = DateTimeOffset.UtcNow;


        var rateLimiterModel = new RateLimiterModel()
        {
            Bucket = 1,
            LastCheck = date,
        };

        var rateLimiterModelString = JsonSerializer.Serialize(rateLimiterModel);

        _rateLimiterRepositoryMock
            .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rateLimiterModelString);

        _dateTimeProviderMock
            .Setup(x => x.GetUtcNow())
            .Returns(date);

        // Act
        var allow = await _rateLimiterService.Allow(userIp, default);

        // Assert
        allow.Should().BeTrue();

        var remainingRequests = 0;
        var newRateLimiterModel = rateLimiterModel with {Bucket = remainingRequests};
        var newRateLimiterModelString = JsonSerializer.Serialize(newRateLimiterModel);

        _rateLimiterRepositoryMock
            .Verify(x => x.Set(userIp, newRateLimiterModelString, It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Allow_NoRequestsLeft_NotAllowed()
    {
        // Arrange
        const string userIp = "192.168.0.0";
        var date = DateTimeOffset.UtcNow;

        var rateLimiterModel = new RateLimiterModel()
        {
            Bucket = 0,
            LastCheck = date,
        };

        var rateLimiterModelString = JsonSerializer.Serialize(rateLimiterModel);

        _rateLimiterRepositoryMock
            .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rateLimiterModelString);

        _dateTimeProviderMock
            .Setup(x => x.GetUtcNow())
            .Returns(date);

        // Act
        var allow = await _rateLimiterService.Allow(userIp, default);

        // Assert
        allow.Should().BeFalse();

        _rateLimiterRepositoryMock
            .Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}