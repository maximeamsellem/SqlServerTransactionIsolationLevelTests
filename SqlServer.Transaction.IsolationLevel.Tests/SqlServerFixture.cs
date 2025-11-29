using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;
using Dapper;
using Xunit.Abstractions;

namespace SqlServer.Transaction.IsolationLevel.Tests;

public class SqlServerFixture : IAsyncLifetime
{
    private Lazy<MsSqlContainer> _sqlContainer = new(new MsSqlBuilder().WithPassword("Strong@Passw0rd").Build);

    public string ConnectionString => _sqlContainer.Value.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _sqlContainer.Value.StartAsync();

        using var connection = new SqlConnection(ConnectionString);

        connection.Open();

        connection.Execute(
            @"CREATE TABLE dbo.Produits (
                Id uniqueidentifier,
                Stock INT
            );");
    }

    public async Task DisposeAsync()
    {
        if (_sqlContainer.IsValueCreated)
        {
            await _sqlContainer.Value.StopAsync();
        }
    }
}
