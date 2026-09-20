using HomeLibrary.Application.Books;
using HomeLibrary.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", () => Results.Redirect("/books"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapControllers();

app.Run();
