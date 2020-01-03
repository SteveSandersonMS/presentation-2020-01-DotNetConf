namespace FlightFinder.Shared
{
    public partial class Itinerary
    {
        public decimal Price
        {
            get => GrpcPrice.ToDecimal();
            set { GrpcPrice = GrpcDecimal.FromDecimal(value); }
        }

        public double TotalDurationHours
            => Outbound.DurationHours + Return.DurationHours;

        public string AirlineName
            => (Outbound.Airline == Return.Airline) ? Outbound.Airline : "Multiple airlines";
    }
}
