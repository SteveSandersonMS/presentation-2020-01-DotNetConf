using Google.Protobuf.WellKnownTypes;
using System;

namespace FlightFinder.Shared
{
    public partial class SearchCriteria
    {
        public SearchCriteria(string fromAirport, string toAirport) : this()
        {
            FromAirport = fromAirport;
            ToAirport = toAirport;
            OutboundDate = DateTime.Now.Date;
            ReturnDate = OutboundDate.AddDays(7);
        }

        public DateTime OutboundDate
        {
            get => OutboundDateTimestamp.ToDateTime();
            set { OutboundDateTimestamp = Timestamp.FromDateTime(value.ToUniversalTime()); }
        }

        public DateTime ReturnDate
        {
            get => ReturnDateTimestamp.ToDateTime();
            set { ReturnDateTimestamp = Timestamp.FromDateTime(value.ToUniversalTime()); }
        }
    }
}
