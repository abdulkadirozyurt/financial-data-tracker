using Company.ClassLibrary1;
using FinancialDataTracker.Business;
using FinancialDataTracker.DataAccess;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.WebAPI.Schedule;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<StockSyncHostedService>();
builder.Services.AddHostedService<QuoteSnapshotSyncHostedService>();

builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddBusinessServices();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await MigrateDatabaseAsync(app.Services);

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapControllers();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

static async Task MigrateDatabaseAsync(IServiceProvider services)
{
    const int maxAttempts = 20;
    var delay = TimeSpan.FromSeconds(5);

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
            return;
        }
        catch when (attempt < maxAttempts)
        {
            await Task.Delay(delay);
        }
    }
}
