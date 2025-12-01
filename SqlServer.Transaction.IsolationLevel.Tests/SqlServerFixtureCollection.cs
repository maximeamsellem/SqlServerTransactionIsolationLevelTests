using Xunit;

namespace SqlServer.Transaction.IsolationLevel.Tests;

[CollectionDefinition(nameof(SqlServerFixture))]
public class SqlServerFixtureCollection : ICollectionFixture<SqlServerFixture> { }