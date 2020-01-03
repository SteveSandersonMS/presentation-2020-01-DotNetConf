using System;
using System.Linq;
using System.Threading.Tasks;
using FlightFinder.Server;
using FlightFinder.Shared;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace FlightFinder.Services
{
    public class FlightDataService : FlightData.FlightDataBase
    {
        public override Task<AirportsReply> Airports(Empty _, ServerCallContext context)
        {
            var reply = new AirportsReply();
            reply.Airports.AddRange(SampleData.Airports);
            return Task.FromResult(reply);
        }

        public override async Task<SearchReply> Search(SearchCriteria criteria, ServerCallContext context)
        {
            await Task.Delay(500); // Gotta look busy...

            var rng = new Random();
            var reply = new SearchReply();
            reply.Itineraries.AddRange(Enumerable.Range(0, rng.Next(1, 5)).Select(_ => new Itinerary
            {
                Price = rng.Next(100, 2000),
                Outbound = new FlightSegment
                {
                    Airline = RandomAirline(),
                    FromAirportCode = criteria.FromAirport,
                    ToAirportCode = criteria.ToAirport,
                    DepartureTime = criteria.OutboundDate.AddHours(rng.Next(24)).AddMinutes(5 * rng.Next(12)),
                    ReturnTime = criteria.OutboundDate.AddHours(rng.Next(24)).AddMinutes(5 * rng.Next(12)),
                    DurationHours = 2 + rng.Next(10),
                    TicketClass = criteria.TicketClass
                },
                Return = new FlightSegment
                {
                    Airline = RandomAirline(),
                    FromAirportCode = criteria.ToAirport,
                    ToAirportCode = criteria.FromAirport,
                    DepartureTime = criteria.ReturnDate.AddHours(rng.Next(24)).AddMinutes(5 * rng.Next(12)),
                    ReturnTime = criteria.ReturnDate.AddHours(rng.Next(24)).AddMinutes(5 * rng.Next(12)),
                    DurationHours = 2 + rng.Next(10),
                    TicketClass = criteria.TicketClass
                },
            }));

            return reply;
        }

        private string RandomAirline()
            => SampleData.Airlines[new Random().Next(SampleData.Airlines.Length)];
    }
}
