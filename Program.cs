using AventusSharp.Data.Storage.Default;
using AventusSharp.Data.Storage.Mysql;
using Serilog;

namespace ${{projectName}};

public class Program
{
    public static readonly string Version = "0.0.1";

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

        Configuration config = new Configuration(builder.Configuration, builder.Environment);
        StorageCredentials storageCredentials = new StorageCredentials(
            host: Config.Database.Host,
            database: Config.Database.Database,
            username: Config.Database.Username,
            password: Config.Database.Password
        )
        {
            port = Config.Database.Port
        };

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services
            .AddSingleton(config)
            .AddSingleton<IDBStorage>(new MySQLStorage(storageCredentials))
            .AddDistributedMemoryCache()
            .AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            })
            .AddSerilog((services, lc) =>
            {
                lc.ReadFrom.Configuration(builder.Configuration)
                    .ReadFrom.Services(services);
            });
        WebApplication app = builder.Build();

        app
            .UseHsts()
            .UseSession()
            .UseCookiePolicy()
            .UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            })
            .UseStaticFiles()
            .UseAventusHttp((config) =>
            {

            })
            .UseAventusWebsocket()
            .UseAventusExport()
            .UseAventusData((config) =>
            {

            })
            .Run();

    }
}
