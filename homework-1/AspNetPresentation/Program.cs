using Core.DependencyInjection.Extensions;
using DataAccess.Configuration;
using DataAccess.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddCore();
builder.Services.AddInfrastructureDataAccess(new FilesConfiguration()
{
    FilePathSalesHistory = "../Files/sales_history.txt",
    FilePathProductRates = "../Files/product_rates.txt"
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();