using HomeLibrary.Application.Books;
using HomeLibrary.Infrastructure.Books;
using HomeLibrary.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeLibrary.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");

        services.AddSingleton<ISqlConnectionFactory>(
            new SqlConnectionFactory(connectionString));
        services.AddScoped<IBookRepository, BookRepository>();

        return services;
    }
}
