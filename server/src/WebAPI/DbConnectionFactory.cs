using Npgsql;
using System.Data;

namespace Cryptic.WebAPI;

public static class DbConnectionFactory
{
    public static async Task<IDbConnection> NewConnectionAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        return conn;
    }
}