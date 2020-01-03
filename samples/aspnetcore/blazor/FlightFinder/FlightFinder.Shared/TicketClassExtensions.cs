using System;

namespace FlightFinder.Shared
{
    public static class TicketClassExtensions
    {
        public static string ToDisplayString(this TicketClass ticketClass)
        {
            switch (ticketClass)
            {
                case TicketClass.Economy: return "Economy";
                case TicketClass.PremiumEconomy: return "Premium Economy";
                case TicketClass.Business: return "Business";
                case TicketClass.First: return "First";
                default: throw new ArgumentException("Unknown ticket class: " + ticketClass.ToString());
            }
        }
    }
}
