using System.Data.Common;

namespace HomeLibrary.Infrastructure.Data;

public interface ISqlConnectionFactory
{
    DbConnection CreateConnection();
}
