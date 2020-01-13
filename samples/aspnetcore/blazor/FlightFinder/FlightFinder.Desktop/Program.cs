using FlightFinder.Client;
using WebWindows.Blazor;
using System.Globalization;
using System.Threading;
using System.IO;
using System;

namespace FlightFinder.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            // Needed for running as native macOS app
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            // Knows how to start up the Blazor app using native .NET Core,
            // supplying DI services, configuration, etc.
            ComponentsDesktop.Run<Startup>("FlightFinder", "wwwroot/index.html");
        }
    }
}
