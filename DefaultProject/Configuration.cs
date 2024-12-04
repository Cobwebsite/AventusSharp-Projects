using System.Reflection;
using AventusSharp.Tools;
using AventusSharp.Tools.Attributes;

namespace ${{projectName}};

public class Configuration
{
    public DatabaseConfig Database { get; private set; }
    public Configuration(WebApplication app)
    {
        Database = GetDatabase(app) ?? throw new Exception("Can't load the Database Config");
    }

    private DatabaseConfig? GetDatabase(WebApplication app)
    {
        return app.Configuration.GetSection("Database").Get<DatabaseConfig>() ?? EnvConfig.Load<DatabaseConfig>();
    }
}


public class DatabaseConfig
{
    [EnvName("MYSQL_HOST")]
    public required string Host { get; set; }
    [EnvName("MYSQL_PORT")]
    public int? Port { get; set; }
    [EnvName("MYSQL_DATABASE")]
    public required string Database { get; set; }
    [EnvName("MYSQL_USERNAME")]
    public required string Username { get; set; }
    [EnvName("MYSQL_PASSWORD")]
    public required string Password { get; set; }


    public override string ToString()
    {
        return $"{Host}:{Port} {Database} {Username} {Password}";
    }
}
