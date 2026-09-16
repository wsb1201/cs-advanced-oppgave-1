using LibrarySystem.Api.Data;
using LibrarySystem.Api.Repositories;
using LibrarySystem.Domain;
using LibrarySystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();

var connectionString =
	builder.Configuration.GetConnectionString("Default")
	?? Environment.GetEnvironmentVariable("ConnectionString_Default");

builder.Services.AddDbContext<LibraryDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<ILibraryRepository, EfLibraryRepository>();
builder.Services.AddScoped<LibraryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}
app.MapControllers();
app.MapHealthChecks("/health");
app.Urls.Add("http://0.0.0.0:80");
app.Run();

public partial class Program { }
