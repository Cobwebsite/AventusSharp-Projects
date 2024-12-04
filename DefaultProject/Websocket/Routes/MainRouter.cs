using AventusSharp.WebSocket;

namespace ${{projectName}}.Websocket.Routes;

public class MainRouter : WsRouter
{
    public string Index()
    {
        return "Websocket is alive";
    }
}