namespace DataAccess.Configuration;

public record FilesConfiguration
{
    public string? FilePathProductRates { get; init; }
    public string? FilePathSalesHistory { get; init; }
}