using Testcontainers.MsSql;
using Xunit;
using Xunit.Abstractions;

namespace SqlServer.Transaction.IsolationLevel.Tests;

[Collection(nameof(SqlServerFixture))]
public class SqlServerContainerTests
{
    private readonly ITestOutputHelper _output;
    private readonly SqlServerFixture _sqlServerFixture;

    public SqlServerContainerTests(ITestOutputHelper output, SqlServerFixture sqlServerFixture)
    {
        _output = output;
        _sqlServerFixture = sqlServerFixture;
    }

    [Fact]
    public void StartSqlServerContainer()
    {
        // Act
        var connectionString = _sqlServerFixture.ConnectionString;

        // Assert
        _output.WriteLine($"Connection string : {connectionString}");
        Assert.NotNull(connectionString);
    }
}
