using HomeLibrary.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "HomeLibrary is running");
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.Run();
