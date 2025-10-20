using AventusSharp.Data;
using AventusSharp.Data.Manager.DB;
using AventusSharp.Data.Storage.Default;
using AventusSharp.Data.Storage.Mysql;
using AventusSharp.Tools;
using AventusSharp.Routes;
using RouterMiddleware = AventusSharp.Routes.RouterMiddleware;
using AventusSharp.WebSocket;

namespace ${{projectName}};

public class Aventus
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static MySQLStorage Storage { get; private set; }
    private static Configuration Config { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public static VoidWithError Init(WebApplication app)
    {
        VoidWithError initResult = new();
        Config = new Configuration(app);
        InitData(initResult);
        InitHttp(initResult, app);
        InitWs(initResult, app);
        return initResult;
    }

    private static void InitData(VoidWithError initResult)
    {
        Storage = new(new StorageCredentials(
            host: Config.Database.Host,
            database: Config.Database.Database,
            username: Config.Database.Username,
            password: Config.Database.Password
        ));
        //Storage.ResetStorage();
        DataMainManager.Configure((config) =>
        {
            config.defaultDM = typeof(SimpleDatabaseDM<>);
            config.defaultStorage = Storage;
        });
        initResult.RunAsync(DataMainManager.Init);
    }

    private static void InitHttp(VoidWithError initResult, WebApplication app)
    {
        RouterMiddleware.Configure((config) =>
        {
        });
        initResult.Run(RouterMiddleware.Register);
        app.Use(RouterMiddleware.OnRequest);
    }

    private static void InitWs(VoidWithError initResult, WebApplication app)
    {
        WebSocketMiddleware.Configure((config) =>
        {
        });
        initResult.Run(WebSocketMiddleware.Register);
        app.Use(WebSocketMiddleware.OnRequest);
    }

    public static void Stop()
    {
        WebSocketMiddleware.Stop().GetAwaiter().GetResult();
    }
}
