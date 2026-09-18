using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace HomeLibrary.Infrastructure.Data;

internal sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public DbConnection CreateConnection() => new SqlConnection(connectionString);
}
