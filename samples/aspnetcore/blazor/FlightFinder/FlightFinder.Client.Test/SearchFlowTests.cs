using Fizzler.Systems.HtmlAgilityPack;
using FlightFinder.Client.Services;
using FlightFinder.Client.Test.TestHelpers;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Components.Testing;
using System;
using System.Linq;
using Xunit;

namespace FlightFinder.Client.Test
{
    public class SearchFlowTests
    {
        private readonly TestHost host = new TestHost();
        private readonly TestGrpcClient<FlightData.FlightDataClient> flightDataClient = new TestGrpcClient<FlightData.FlightDataClient>();

        public SearchFlowTests()
        {
            host.AddService(flightDataClient.Client);
            host.AddService(new AppState(flightDataClient.Client));
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
            Assert.Empty(flightDataClient.Calls<SearchCriteria>());

            // Act
            app.Find(".search-submit").Click();
            var call = flightDataClient.Calls<SearchCriteria>().Single();
            var searchCriteria = call.Request;

            // Assert
            Assert.Equal("JFK", searchCriteria.FromAirport);
            Assert.Equal("SYD", searchCriteria.ToAirport);
            Assert.Equal(new DateTime(2020, 1, 2), searchCriteria.OutboundDate);
            Assert.Equal(new DateTime(2020, 3, 8), searchCriteria.ReturnDate);
            Assert.Equal(TicketClass.First, searchCriteria.TicketClass);
        }

        [Fact]
        public void ShowsSearchResults()
        {
            // Arrange: Prepare a reply from server
            var app = host.AddComponent<Main>();
            var reply = new SearchReply();
            reply.Itineraries.Add(TestItinerary.Create("My Airline", 123));
            reply.Itineraries.Add(TestItinerary.Create("Another Airline", 456m));

            // Act: Click 'search'; receive reply
            app.Find(".search-submit").Click();
            host.WaitForNextRender(() => flightDataClient.SetResponse(reply));

            // Assert: Verify UI displays the data
            Assert.Equal("2 results", app.Find("#results-area .title h2").InnerText);
            Assert.Collection(app.FindAll("#results-area .search-result"),
                item =>
                {
                    Assert.Equal("$123", item.QuerySelector(".price h3").InnerText);
                    Assert.Equal("My Airline", item.QuerySelector(".airline span").InnerText);
                },
                item =>
                {
                    Assert.Equal("$456", item.QuerySelector(".price h3").InnerText);
                    Assert.Equal("Another Airline", item.QuerySelector(".airline span").InnerText);
                });
        }

        [Fact]
        public void GreysOutSearchResultsWhileLoading()
        {
            var app = host.AddComponent<Main>();
            Func<bool> isGreyedOut = () => app.Find("#results-area").HasClass("greyout");

            // Assert: Before clicking search, we're not greyed out
            Assert.False(isGreyedOut());

            // Act/Assert: After clicking search, we are greyed out
            app.Find(".search-submit").Click();
            Assert.True(isGreyedOut());

            // Act/Assert: When the response arrives, we are no longer greyed out
            host.WaitForNextRender(() => flightDataClient.SetResponse(new SearchReply()));
            Assert.False(isGreyedOut());
        }
    }
}
