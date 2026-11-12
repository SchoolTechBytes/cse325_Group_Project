using Npgsql;
using WheresOurDrummer.Web.Data;

namespace WheresOurDrummer.Tests;

public class ConnectionStringResolverTests
{
    [Fact]
    public void FromDatabaseUrl_ParsesPartsAndDefaultsToSslPrefer()
    {
        var cs = new NpgsqlConnectionStringBuilder(
            ConnectionStringResolver.FromDatabaseUrl("postgres://fakeuser:fake%40pass@db.example.test:6543/fakedb"));

        Assert.Equal("db.example.test", cs.Host);
        Assert.Equal(6543, cs.Port);
        Assert.Equal("fakedb", cs.Database);
        Assert.Equal("fakeuser", cs.Username);
        Assert.Equal("fake@pass", cs.Password);
        Assert.Equal(SslMode.Prefer, cs.SslMode);
    }

    [Fact]
    public void FromDatabaseUrl_UsesDefaultPort_AndHonorsSslModeAndPostgresqlScheme()
    {
        var cs = new NpgsqlConnectionStringBuilder(
            ConnectionStringResolver.FromDatabaseUrl("postgresql://u:p@db.example.test/fakedb?sslmode=require"));

        Assert.Equal(5432, cs.Port);
        Assert.Equal(SslMode.Require, cs.SslMode);
    }

    [Theory]
    [InlineData("mysql://u:p@host/db")]
    [InlineData("not a url")]
    public void FromDatabaseUrl_RejectsOtherInput(string input)
    {
        Assert.Throws<FormatException>(() => ConnectionStringResolver.FromDatabaseUrl(input));
    }
}
