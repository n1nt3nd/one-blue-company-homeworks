using System.Collections.Concurrent;
using System.Threading.Channels;
using CsvHelper;
using hw_3.Models;
using hw_3.Services;

namespace hw_3;

public class FileConsumer
{
    private int _amountThreads;
    private readonly IDemandService _demandService;
    private int _calculatedDemandCount = 0;
    private int _wroteProductCount = 0;
    private readonly ConcurrentQueue<Task> _consumers;
    private readonly Dictionary<Task, CancellationTokenSource> _cancellationTokenSourceFromTask;
    private readonly ChannelReader<ProductStatistics> _channelReader;
    private readonly CsvWriter _csvWriter;

    private readonly SemaphoreSlim _writeFileSemaphore = new(1, 1);

    public FileConsumer(int amountThreads, IDemandService demandService, ChannelReader<ProductStatistics> channelReader,
        CsvWriter csvWriter)
    {
        _amountThreads = amountThreads;
        _demandService = demandService;
        _channelReader = channelReader;
        _csvWriter = csvWriter;

        _cancellationTokenSourceFromTask = new Dictionary<Task, CancellationTokenSource>();
        _consumers = new ConcurrentQueue<Task>();
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        for (var i = 0; i < _amountThreads; i++)
        {
            AddTask();
        }

        cancellationToken.Register(() =>
        {
            CancelAllTasks();
            cancellationToken.ThrowIfCancellationRequested();
        });

        await Task.WhenAll(_consumers);
    }

    public void ChangeAmountThreads(int newAmountThreads)
    {
        if (newAmountThreads > _amountThreads)
        {
            while (_consumers.Count != newAmountThreads)
            {
                AddTask();
            }
        }
        else
        {
            while (_consumers.Count != newAmountThreads)
            {
                RemoveTask();
            }
        }

        _amountThreads = newAmountThreads;
    }

    private void CancelAllTasks()
    {
        while (!_consumers.IsEmpty)
        {
            RemoveTask();
        }
    }

    private void AddTask()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        var task = Task.Run(() => NewConsumerInChannel(token), token);
        _consumers.Enqueue(task);
        _cancellationTokenSourceFromTask.Add(task, cts);
    }

    private void RemoveTask()
    {
        if (!_consumers.TryDequeue(out var task))
            return;

        var cts = _cancellationTokenSourceFromTask[task];
        cts.Cancel();
        _cancellationTokenSourceFromTask.Remove(task);
    }

    private async Task NewConsumerInChannel(CancellationToken cancellationToken)
    {
        while (await _channelReader.WaitToReadAsync())
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (!_channelReader.TryRead(out var productStatistics)) continue;
            var demand = await _demandService.Calculate(productStatistics.Prediction, productStatistics.Stock);
            Interlocked.Increment(ref _calculatedDemandCount);
            PrintStatistics();

            var productDemand = new ProductDemand()
            {
                Id = productStatistics.Id,
                Demand = demand,
            };

            await _writeFileSemaphore.WaitAsync();
            _csvWriter.WriteRecord(productDemand);
            await _csvWriter.NextRecordAsync();
            _writeFileSemaphore.Release();

            Interlocked.Increment(ref _wroteProductCount);

            PrintStatistics();
        }
    }

    private void PrintStatistics()
    {
        Console.WriteLine(
            $"Calculated Products {_calculatedDemandCount}, Wrote Products {_wroteProductCount}");
    }
}