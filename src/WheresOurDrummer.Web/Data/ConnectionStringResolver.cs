using Npgsql;

namespace WheresOurDrummer.Web.Data;

/// <summary>Builds the Npgsql connection string: DATABASE_URL (Render style) wins, else appsettings.</summary>
public static class ConnectionStringResolver
{
    public static string Resolve(IConfiguration configuration)
    {
        var url = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(url))
        {
            return FromDatabaseUrl(url);
        }

        return configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No database configured. Set DATABASE_URL or ConnectionStrings:DefaultConnection.");
    }

    /// <summary>Parses postgres://user:pass@host:port/db?sslmode=require into an Npgsql connection string.</summary>
    public static string FromDatabaseUrl(string databaseUrl)
    {
        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != "postgres" && uri.Scheme != "postgresql"))
        {
            // Deliberately do not echo the value: it contains credentials.
            throw new FormatException("DATABASE_URL must be a postgres:// or postgresql:// URI.");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort || uri.Port < 0 ? 5432 : uri.Port,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
            SslMode = SslMode.Prefer,
        };

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            var parts = uri.UserInfo.Split(':', 2);
            builder.Username = Uri.UnescapeDataString(parts[0]);
            if (parts.Length > 1)
            {
                builder.Password = Uri.UnescapeDataString(parts[1]);
            }
        }

        foreach (var pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split('=', 2);
            if (kv.Length == 2 && kv[0].Equals("sslmode", StringComparison.OrdinalIgnoreCase)
                && Enum.TryParse<SslMode>(Uri.UnescapeDataString(kv[1]).Replace("-", ""), ignoreCase: true, out var mode))
            {
                builder.SslMode = mode;
            }
        }

        return builder.ConnectionString;
    }
}
