using Microsoft.EntityFrameworkCore;
using SneakerStore.Application.Services;
using SneakerStore.Core.Interfaces.Repositories.Sneaker;
using SneakerStore.FailureHandler;
using SneakerStore.Persistence;
using SneakerStore.Persistence.Repositories.Sneaker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connecting db
builder.Services.AddDbContext<SneakerStoreDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(SneakerStoreDbContext)));
});

// Repositories
builder.Services.AddScoped<ISneakerRepository, SneakerRepository>();

// Services
builder.Services.AddScoped<ISneakerService, SneakerService>();

// Failure Handlers
builder.Services.AddScoped<IFailureHandler, FailureHandler>();

builder.Services.AddProblemDetails();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
