using System.Threading.Tasks;
using Core.Services;

namespace ConsolePresentation.Commands;

using Spectre.Console;
using Spectre.Console.Cli;

public class AdsCommand : AsyncCommand<AdsCommand.Settings>
{
    private readonly IAdsService _adsService;

    public AdsCommand(IAdsService adsService)
    {
        _adsService = adsService;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<ProductId>")] public long ProductId { get; init; }
    }


    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.WriteLine(await _adsService.CalculateAsync(settings.ProductId));

        return 0;
    }
}