
namespace ${{projectName}};

public class Configuration : AutoConfiguration
{
    public DatabaseConfig Database { get; private set; } = null!;

    [ConfigIgnore]
    public string EnvironmentName { get; private set; } = "";

    public Configuration(IConfiguration config, IHostEnvironment environment) : base(config)
    {
        EnvironmentName = environment.EnvironmentName;
        if (!AventusExtension.IsExportCommand)
        {
            Validate(environment);
        }
    }

    private void Validate(IHostEnvironment environment)
    {
        List<string> errors = [];

        Require(Database.Host, "Database.Host", errors);
        Require(Database.Database, "Database.Database", errors);
        Require(Database.Username, "Database.Username", errors);
        ValidatePort(Database.Port, "Database.Port", errors);

        if (environment.IsProduction())
        {
            Require(Database.Password, "Database.Password", errors);
        }

        if (errors.Count > 0) throw new ConfigurationValidationException(errors);
    }


    private static void Require(string value, string key, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value)) errors.Add($"{key} is required.");
    }

    private static void ValidatePort(uint? port, string key, List<string> errors)
    {
        if (port is < 1 or > 65535) errors.Add($"{key} must be between 1 and 65535.");
    }
}

public sealed class ConfigurationValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ConfigurationValidationException(IEnumerable<string> errors)
        : base("Invalid application configuration:" + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(error => $" - {error}")))
    {
        Errors = errors.ToList().AsReadOnly();
    }
}

public sealed class DatabaseConfig
{
    public string Host { get; set; } = "";

    public uint? Port { get; set; }

    public string Database { get; set; } = "";

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";
}
