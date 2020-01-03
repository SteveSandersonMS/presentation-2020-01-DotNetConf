using FlightFinder.Shared;

namespace FlightFinder.Client.Test.TestHelpers
{
    public class TestItinerary
    {
        public static Itinerary Create(string airline, decimal price) => new Itinerary
        {
            Price = price,
            Outbound = new FlightSegment { Airline = airline, DepartureTime = default, ReturnTime = default },
            Return = new FlightSegment { Airline = airline, DepartureTime = default, ReturnTime = default },
        };
    }
}
