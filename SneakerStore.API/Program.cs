using Microsoft.EntityFrameworkCore;
using SneakerStore.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SneakerStoreDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(SneakerStoreDbContext)));
});

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
