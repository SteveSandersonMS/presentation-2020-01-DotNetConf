using Fizzler.Systems.HtmlAgilityPack;
using FlightFinder.Client.Components;
using FlightFinder.Client.Test.TestHelpers;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using System.Collections.Generic;
using Xunit;

namespace FlightFinder.Client.Test
{
    public class ShortlistTests
    {
        private readonly TestHost host = new TestHost();

        [Fact]
        public void CanDisplayEmpty()
        {
            var component = host.AddComponent<Shortlist>(new Dictionary<string, object>
            {
                [nameof(Shortlist.Itineraries)] = new List<Itinerary>()
            });

            Assert.Equal("Shortlist (0)", component.Find("h2").InnerText);
            Assert.Empty(component.FindAll("ul"));
        }

        [Fact]
        public void RoundsPricesToNearestUnit()
        {
            var component = host.AddComponent<Shortlist>(new Dictionary<string, object>
            {
                [nameof(Shortlist.Itineraries)] = new List<Itinerary>
                {
                    TestItinerary.Create("Fake Airways", price: 123.45m), // Round down
                    TestItinerary.Create("Test Airways", price: 456.78m), // Round up
                }
            });

            Assert.Equal("Shortlist (2)", component.Find("h2").InnerText);
            Assert.Collection(component.FindAll("ul"),
                item =>
                {
                    Assert.Equal("Fake Airways", item.QuerySelector(".airline").InnerText);
                    Assert.Equal("$123", item.QuerySelector(".price").InnerText);
                },
                item =>
                {
                    Assert.Equal("Test Airways", item.QuerySelector(".airline").InnerText);
                    Assert.Equal("$457", item.QuerySelector(".price").InnerText);
                });
        }

        [Fact]
        public void TriggersRemoveFromShortlistCallback()
        {
            Itinerary itineraryToRemove = null;
            var component = host.AddComponent<Shortlist>(new Dictionary<string, object>
            {
                [nameof(Shortlist.Itineraries)] = new List<Itinerary>
                {
                    TestItinerary.Create("Fake Airways", price: 123.45m),
                    TestItinerary.Create("Test Airways", price: 456.78m),
                },
                [nameof(Shortlist.OnRemoveItinerary)] = EventCallback.Factory.Create<Itinerary>(this, itinerary =>
                {
                    itineraryToRemove = itinerary;
                })
            });

            // Until we click a 'remove' button, no callback was fired
            Assert.Null(itineraryToRemove);

            // When we click 'remove' on a certain item, the callback fires for that item
            component.Find("ul:nth-last-child(1) .close").Click();
            Assert.NotNull(itineraryToRemove);
            Assert.Equal("Test Airways", itineraryToRemove.AirlineName);
        }
    }
}
