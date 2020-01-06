using FlightFinder.Client.Services;
using FlightFinder.Shared;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace FlightFinder.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(services =>
            {
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));

                // If there's an appsettings.json, get URI from there. Otherwise take it from browser.
                var baseUri = services.GetService<IConfiguration>()?.GetValue<string>("BackendUri")
                           ?? services.GetRequiredService<NavigationManager>().BaseUri;

                var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
                return new FlightData.FlightDataClient(channel);
            });

            services.AddSingleton<AppState>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Main>("body");
        }
    }
}
