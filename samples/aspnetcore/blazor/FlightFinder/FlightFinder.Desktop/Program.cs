using FlightFinder.Client;
using WebWindows.Blazor;

namespace FlightFinder.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            // Knows how to start up the Blazor app using native .NET Core,
            // supplying DI services, configuration, etc.
            ComponentsDesktop.Run<Startup>("FlightFinder", "wwwroot/index.html");
        }
    }
}
