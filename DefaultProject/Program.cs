using AventusSharp.Tools;
using ZstdSharp.Unsafe;

namespace ${{projectName}};

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services
            .AddDistributedMemoryCache()
            .AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
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
            .UseStaticFiles();


        app.Lifetime.ApplicationStopping.Register(Aventus.Stop);

        VoidWithError initResult = Aventus.Init(app);
        if (initResult.Success)
            app.Run();
        else
            initResult.Print();

    }
}
