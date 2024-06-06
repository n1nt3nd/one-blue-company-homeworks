using System.Globalization;
using System.Threading.Channels;
using CsvHelper;
using CsvHelper.Configuration;
using hw_3.Config;
using hw_3.Models;
using hw_3.Services;
using Microsoft.Extensions.Options;

namespace hw_3;

public class FileProcessor
{
    private readonly IOptionsMonitor<ThreadsConfig> _threadsConfig;
    private readonly IOptions<FilesConfig> _filesConfig;
    private readonly IDemandService _demandService;
    private readonly Channel<ProductStatistics> _channel;
    private FileProducer _fileProducer;
    private FileConsumer _fileConsumer;
    private const int ChannelCapacity = 100;

    public FileProcessor(IOptionsMonitor<ThreadsConfig> threadsConfig, IOptions<FilesConfig> filesConfig,
        IDemandService demandService)
    {
        _threadsConfig = threadsConfig;
        _filesConfig = filesConfig;
        _demandService = demandService;
        _channel = Channel.CreateBounded<ProductStatistics>(ChannelCapacity);
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(_filesConfig.Value.InputFile);
        using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        });

        await using var writer = new StreamWriter(_filesConfig.Value.OutputFile);
        await using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        _fileProducer = new FileProducer(_channel.Writer, csvReader);
        _fileConsumer = new FileConsumer(_threadsConfig.CurrentValue.MaxAmountThreads, _demandService, _channel.Reader,
            csvWriter);

        var producer = _fileProducer.Run(cancellationToken);
        var consumer = _fileConsumer.Run(cancellationToken);

        _threadsConfig.OnChange(setting =>
        {
            _fileConsumer.ChangeAmountThreads(setting.MaxAmountThreads);
        });

        await Task.WhenAll(producer, consumer);
    }
}