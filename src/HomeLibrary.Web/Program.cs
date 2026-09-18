var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/", () => "HomeLibrary is running");
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.Run();
