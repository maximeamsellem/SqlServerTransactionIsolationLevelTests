using Testcontainers.MsSql;
using Xunit;
using Xunit.Abstractions;

namespace SqlServer.Transaction.IsolationLevel.Tests;

public class SqlServerContainerTests : IAsyncDisposable
{
    private readonly ITestOutputHelper _output;
    private MsSqlContainer _sqlContainer;

    public SqlServerContainerTests(ITestOutputHelper output) => _output = output;

    public async ValueTask DisposeAsync()
    {
        if (_sqlContainer != null)
        {
            await _sqlContainer.StopAsync();
        }
    }

    [Fact]
    public async Task StartSqlServerContainer()
    {
        // Arrange
        var sqlContainerBuilder = new MsSqlBuilder().WithPassword("Strong@Passw0rd");

        _sqlContainer = sqlContainerBuilder.Build();

        await _sqlContainer.StartAsync();

        _output.WriteLine("SQL Server container started.");
        _output.WriteLine($"Connection string : {_sqlContainer.GetConnectionString()}");

        // Act
        var connectionString = _sqlContainer.GetConnectionString();

        // Assert
        Assert.NotNull(connectionString);
    }
}
