using System.Threading.Tasks;
using Core.Services;

namespace ConsolePresentation.Commands;

using Spectre.Console;
using Spectre.Console.Cli;

public class SalesPredictionCommand : AsyncCommand<SalesPredictionCommand.Settings>
{
    private readonly ISalesPredictionService _salesPredictionService;

    public SalesPredictionCommand(ISalesPredictionService salesPredictionService)
    {
        _salesPredictionService = salesPredictionService;
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
        AnsiConsole.WriteLine(await _salesPredictionService.CalculateAsync(settings.ProductId, settings.DaysAmount));

        return 0;
    }
}