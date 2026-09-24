using AventusSharp.Data.Storage.Default;
using AventusSharp.Data.Storage.${{db}};
using Serilog;

namespace ${{projectName}};

public class Program
{
    public static readonly string Version = "0.0.1";

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        Configuration configuration = new Configuration(builder.Configuration, builder.Environment);
        IDBStorage storage = new ${{storage}}(configuration.Database);

        builder.Services
            .AddSingleton(configuration)
            .AddSingleton(storage)
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

            });

        app.Run();

    }
}
