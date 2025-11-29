using Microsoft.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;
using Dapper;

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
    public void ConnectionString_Should_Not_Be_Null()
    {
        // Act
        var connectionString = _sqlServerFixture.ConnectionString;

        // Assert
        _output.WriteLine($"Connection string : {connectionString}");
        Assert.NotNull(connectionString);
    }

    [Fact]
    public async Task Read_Uncommited_Should_Get_Uncommited_Data_When_Update_Transaction_Is_Not_Committed_Yet()
    {
        // Arrange
        using var connection1 = GetSqlConnection();
        using var connection2 = GetSqlConnection();

        var initialStock = 100;
        var updatedStock = 90;
        var productId = Guid.NewGuid();

        await InsertProduitAsync(connection1, productId, initialStock);

        using var updateTransaction = connection1.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        await UpdateProduitAsync(connection1, updateTransaction, productId, updatedStock);

        // Act       
        var stock = await QuerySingleWithReadUncommittedAsync(connection2, productId);
        await updateTransaction.CommitAsync();

        // Assert
        Assert.Equal(updatedStock, stock);
    }

    private async Task InsertProduitAsync(SqlConnection connection, Guid productId, int stock)
    {
        using var transaction = connection.BeginTransaction();

        _output.WriteLine($"InsertProduitAsync : {productId}");

        await connection.ExecuteAsync(
            "INSERT INTO dbo.Produits VALUES (@Id, @Stock);",
            new { Id = productId, Stock = stock },
            transaction: transaction);

        await transaction.CommitAsync();
    }

    private async Task UpdateProduitAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid productId,
        int newStock)
    {
        _output.WriteLine($"UpdateProduitAsync : {productId}");

        await connection.ExecuteAsync(
            "UPDATE Produits SET Stock = @NewStock WHERE Id = @Id",
            new { Id = productId, NewStock = newStock },
            transaction: transaction);
    }

    private Task<int> QuerySingleWithReadUncommittedAsync(SqlConnection connection, Guid productId)
    {
        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

        _output.WriteLine($"QuerySingleAsync : {productId}");

        return connection.QuerySingleAsync<int>(
            "SELECT Stock FROM Produits WHERE Id = @Id",
            new { Id = productId },
            transaction: transaction
        );
    }

    private SqlConnection GetSqlConnection()
    {
        var connectionString = $"{_sqlServerFixture.ConnectionString};MultipleActiveResultSets=True;";
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
