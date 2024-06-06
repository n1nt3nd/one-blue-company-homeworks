using System.Threading.Channels;
using CsvHelper;
using hw_3.Models;

namespace hw_3;

public class FileProducer
{
    private int _readLinesCount = 0;
    private readonly ChannelWriter<ProductStatistics> _channelWriter;
    private readonly CsvReader _csvReader;

    public FileProducer(ChannelWriter<ProductStatistics> channelWriter, CsvReader csvReader)
    {
        _channelWriter = channelWriter;
        _csvReader = csvReader;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        while (await _csvReader.ReadAsync())
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await Task.Delay(200, cancellationToken);
            var productStatistics = _csvReader.GetRecord<ProductStatistics>();
            Interlocked.Increment(ref _readLinesCount);
            PrintStatistics();

            await _channelWriter.WriteAsync(productStatistics, cancellationToken);
        }

        _channelWriter.Complete();
    }

    private void PrintStatistics()
    {
        Console.WriteLine($"Read lines  {_readLinesCount}");
    }
}