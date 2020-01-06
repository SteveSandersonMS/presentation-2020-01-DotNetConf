using FlightFinder.Client;
using WebWindows.Blazor;

namespace FlightFinder.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            ComponentsDesktop.Run<Startup>("FlightFinder", "wwwroot/index.html");
        }
    }
}
