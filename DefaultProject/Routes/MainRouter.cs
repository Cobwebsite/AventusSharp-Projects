using AventusSharp.Routes;
using AventusSharp.Routes.Response;

namespace ${{projectName}}.Routes;

public class MainRouter : Router
{
    public View Index()
    {
        return new View("index");
    }
}
