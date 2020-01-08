using Google.Protobuf.WellKnownTypes;
using System;

namespace FlightFinder.Shared
{
    public partial class FlightSegment
    {
        public DateTime DepartureTime
        {
            get => DepartureTimestamp.ToDateTime();
            set { DepartureTimestamp = Timestamp.FromDateTime(value.ToUniversalTime()); }
        }

        public DateTime ReturnTime
        {
            get => ReturnTimestamp.ToDateTime();
            set { ReturnTimestamp = Timestamp.FromDateTime(value.ToUniversalTime()); }
        }
    }
}
