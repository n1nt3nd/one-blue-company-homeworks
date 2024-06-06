using ConsolePresentation.Commands;
using Core.Services;
using Spectre.Console.Cli;

namespace ConsolePresentation.Extensions;

public static class CommandAppExtensions
{
    public static ICommandApp AddCommands(this ICommandApp app)
    {
        app.Configure(config =>
        {
            config.AddCommand<AdsCommand>("ads");
            config.AddCommand<SalesPredictionCommand>("prediction");
            config.AddCommand<DemandCommand>("demand");
        });        

        return app;
    }
}