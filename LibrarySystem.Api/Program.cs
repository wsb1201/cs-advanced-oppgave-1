using LibrarySystem.Api.Data;
using LibrarySystem.Api.Repositories;
using LibrarySystem.Domain;
using LibrarySystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenApi(options =>
{
	options.AddDocumentTransformer(
		(document, _, _) =>
		{
			document.Components ??= new OpenApiComponents();
			document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>
			{ };

			document.Components.SecuritySchemes["bearer"] = new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				Description = "JWT authorization token",
			};

			return Task.CompletedTask;
		}
	);
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
	options.UseNpgsql(
		connectionString: builder.Configuration.GetConnectionString("Default")
			?? Environment.GetEnvironmentVariable("ConnectionString_Default")
	)
);
builder.Services.AddScoped<ILibraryRepository, EfLibraryRepository>();
builder.Services.AddScoped<LibraryService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference(options => options.AddPreferredSecuritySchemes("bearer"));
}
app.MapControllers();
app.MapHealthChecks("/health");
app.Urls.Add("http://0.0.0.0:80");

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
	await db.Database.MigrateAsync();

	var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();
	var username = Environment.GetEnvironmentVariable("INITIAL_ADMIN_USER");
	var password = Environment.GetEnvironmentVariable("INITIAL_ADMIN_PASSWORD");
	if (
		!string.IsNullOrWhiteSpace(username)
		&& !string.IsNullOrWhiteSpace(password)
		&& await repo.AddUser(
			name: username,
			hash: BCrypt.Net.BCrypt.HashPassword(password),
			librarian: true
		)
	)
	{
		Console.WriteLine($"Created default admin user \"{username}\"");
	}
}

await app.RunAsync();

public partial class Program { }
