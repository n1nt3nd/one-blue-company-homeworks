using System;
using ConsolePresentation.Extensions;
using Core.DependencyInjection.Extensions;
using DataAccess.Configuration;
using DataAccess.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

var collection = new ServiceCollection();

collection.AddCore();
collection.AddInfrastructureDataAccess(new FilesConfiguration()
{    
    FilePathSalesHistory = "../../../../Files/sales_history.txt",
    FilePathProductRates = "../../../../Files/product_rates.txt"
});

var provider = collection.BuildServiceProvider();
using var scope = provider.CreateScope();

using var registrar = new DependencyInjectionRegistrar(collection);
var app = new CommandApp(registrar);

app.AddCommands();

while (true)
{
    args = Console.ReadLine().Split();
    app.Run(args);
}