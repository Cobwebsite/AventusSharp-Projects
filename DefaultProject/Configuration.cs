using System.Reflection;

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

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EnvName : Attribute
{
    internal string Name { get; private set; }
    public EnvName(string name)
    {
        Name = name;
    }
}
public class EnvConfig
{
    public static T? Load<T>()
    {
        Type t = typeof(T);
        T? config = (T?)Activator.CreateInstance(t);

        if (config == null) return default;

        PropertyInfo[] properties = t.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            EnvName? envName = property.GetCustomAttribute<EnvName>();
            if (envName != null)
            {
                string? envValue = Environment.GetEnvironmentVariable(envName.Name);
                Type valueType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                object? convertedValue = envValue != null ? Convert.ChangeType(envValue, valueType) : null;
                property.SetValue(config, convertedValue);
            }
        }

        FieldInfo[] fields = t.GetFields();
        foreach (FieldInfo field in fields)
        {
            EnvName? envName = field.GetCustomAttribute<EnvName>();
            if (envName != null)
            {
                string? envValue = Environment.GetEnvironmentVariable(envName.Name);
                Type valueType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                object? convertedValue = envValue != null ? Convert.ChangeType(envValue, valueType) : null;
                field.SetValue(config, convertedValue);
            }
        }

        return config;
    }
}