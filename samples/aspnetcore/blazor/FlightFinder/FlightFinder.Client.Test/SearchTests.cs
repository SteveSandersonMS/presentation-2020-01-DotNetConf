using Fizzler.Systems.HtmlAgilityPack;
using FlightFinder.Client.Services;
using FlightFinder.Client.Test.TestHelpers;
using FlightFinder.Shared;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Testing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FlightFinder.Client.Test
{
    public class SearchTests
    {
        private readonly TestHost host = new TestHost();
        private readonly TestFlightDataClient flightDataClient = new TestFlightDataClient();

        public SearchTests()
        {
            host.AddService<FlightData.FlightDataClient>(flightDataClient);
            host.AddService(new AppState(flightDataClient));
        }

        [Fact]
        public void SendsSearchCriteria()
        {
            // Arrange
            var app = host.AddComponent<Main>();
            app.Find(".search-from-airport input").Change("JFK");
            app.Find(".search-to-airport input").Change("SYD");
            app.Find(".search-outbound-date input").Change("2020-01-02");
            app.Find(".search-return-date input").Change("2020-03-08");
            app.Find(".search-ticket-class select").Change(TicketClass.First.ToString());
            Assert.Null(flightDataClient.LatestSearchCriteria);

            // Act
            app.Find(".search-submit").Click();

            // Assert
            var sentCriteria = flightDataClient.LatestSearchCriteria;
            Assert.Equal("JFK", sentCriteria.FromAirport);
            Assert.Equal("SYD", sentCriteria.ToAirport);
            Assert.Equal(new DateTime(2020, 1, 2), sentCriteria.OutboundDate);
            Assert.Equal(new DateTime(2020, 3, 8), sentCriteria.ReturnDate);
            Assert.Equal(TicketClass.First, sentCriteria.TicketClass);
        }

        [Fact]
        public void ShowsSearchResults()
        {
            // Arrange
            var app = host.AddComponent<Main>();
            var reply = new SearchReply();
            reply.Itineraries.Add(TestItinerary.Create("My Airline", 123.45m));
            reply.Itineraries.Add(TestItinerary.Create("Another Airline", 456.78m));

            // Act
            app.Find(".search-submit").Click();
            host.WaitForNextRender(() => flightDataClient.SearchReply.SetResult(reply));

            // Assert
            Assert.Equal("2 results", app.Find("#results-area .title h2").InnerText);
            Assert.Collection(app.FindAll("#results-area .search-result"),
                item =>
                {
                    Assert.Equal("$123", item.QuerySelector(".price h3").InnerText);
                    Assert.Equal("My Airline", item.QuerySelector(".airline span").InnerText);
                },
                item =>
                {
                    Assert.Equal("$457", item.QuerySelector(".price h3").InnerText);
                    Assert.Equal("Another Airline", item.QuerySelector(".airline span").InnerText);
                });
        }

        class TestFlightDataClient : FlightData.FlightDataClient
        {
            // Captures calls to AirportsAsync
            public TaskCompletionSource<AirportsReply> AirportsReply { get; } = new TaskCompletionSource<AirportsReply>();
            public override AsyncUnaryCall<AirportsReply> AirportsAsync(Empty _, CallOptions options)
                => this.AsyncUnaryCall(AirportsReply.Task);

            // Captures calls to SearchAsync
            public SearchCriteria LatestSearchCriteria { get; private set; }
            public TaskCompletionSource<SearchReply> SearchReply { get; } = new TaskCompletionSource<SearchReply>();
            public override AsyncUnaryCall<SearchReply> SearchAsync(SearchCriteria request, CallOptions options)
            {
                LatestSearchCriteria = request;
                return this.AsyncUnaryCall(SearchReply.Task);
            }
        }
    }
}
