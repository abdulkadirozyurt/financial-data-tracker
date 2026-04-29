using Company.ClassLibrary1;
using FinancialDataTracker.Business;
using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.Business.Concrete;
using FinancialDataTracker.DataAccess;
using FinancialDataTracker.WebAPI.Schedule;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<StockSyncHostedService>();

builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddBusinessServices();




builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapControllers();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Run();

