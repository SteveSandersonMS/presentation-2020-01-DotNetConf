using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Components;

namespace FlightFinder.Client.Services
{
    public class AppState
    {
        // Actual state
        public IReadOnlyList<Itinerary> SearchResults { get; private set; }
        public bool SearchInProgress { get; private set; }

        private readonly List<Itinerary> shortlist = new List<Itinerary>();
        public IReadOnlyList<Itinerary> Shortlist => shortlist;

        // Lets components receive change notifications
        // Could have whatever granularity you want (more events, hierarchy...)
        public event Action OnChange;

        // Receive 'flightDataClient' instance from DI
        private readonly FlightData.FlightDataClient flightDataClient;
        public AppState(FlightData.FlightDataClient flightDataClient)
        {
            this.flightDataClient = flightDataClient;
        }

        public async Task Search(SearchCriteria criteria)
        {
            SearchInProgress = true;
            NotifyStateChanged();

            SearchResults = (await flightDataClient.SearchAsync(criteria)).Itineraries;
            SearchInProgress = false;
            NotifyStateChanged();
        }

        public void AddToShortlist(Itinerary itinerary)
        {
            shortlist.Add(itinerary);
            NotifyStateChanged();
        }

        public void RemoveFromShortlist(Itinerary itinerary)
        {
            shortlist.Remove(itinerary);
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
