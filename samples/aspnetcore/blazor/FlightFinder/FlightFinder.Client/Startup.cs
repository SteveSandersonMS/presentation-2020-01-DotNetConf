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
            services.AddSingleton<AppState>();

            // Add FlightDataClient as DI service
            services.AddSingleton(services =>
            {
                // Create a gRPC-Web channel pointing to the backend server
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));

                // When on desktop, take baseUri from appsettings.json
                // When in browser, take baseUri from browser
                var baseUri = services.GetService<IConfiguration>()?.GetValue<string>("BackendUri")
                           ?? services.GetRequiredService<NavigationManager>().BaseUri;

                var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });

                // Now we can instantiate gRPC clients for this channel
                return new FlightData.FlightDataClient(channel);
            });
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Main>("body");
        }
    }
}
