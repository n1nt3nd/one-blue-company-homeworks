using Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsolePresentation.Commands;

public class DemandCommand : AsyncCommand<DemandCommand.Settings>
{
    private readonly IDemandService _demandService;

    public DemandCommand(IDemandService demandService)
    {
        _demandService = demandService;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<ProductId>")] 
        public long ProductId { get; init; }

        [CommandArgument(1, "<DaysAmount>")]
        public int DaysAmount { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.WriteLine(await _demandService.CalculateAsync(settings.ProductId, settings.DaysAmount));

        return 0;
    }
}