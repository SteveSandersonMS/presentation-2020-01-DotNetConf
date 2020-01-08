namespace FlightFinder.Shared
{
    public partial class GrpcDecimal
    {
        private const decimal NanoFactor = 1_000_000_000;

        public static GrpcDecimal FromDecimal(decimal value)
        {
            var units = decimal.ToInt64(value);
            var nanos = decimal.ToInt32((value - units) * NanoFactor);
            return new GrpcDecimal { Units = units, Nanos = nanos };
        }

        public decimal ToDecimal()
        {
            return Units + Nanos / NanoFactor;
        }
    }
}
