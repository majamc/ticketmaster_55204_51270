namespace ConcertTracker.Models
{
    using Newtonsoft.Json;

    namespace TicketmasterAPI.Models
    {
        /// <summary>
        /// Reprezentuje wydarzenie (np. koncert) w systemie Ticketmaster.
        /// </summary>
        public class EventModel //glowne dane koncertu
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty; //nazwa zespolu/wydarzenia

            public string Date { get; set; } = string.Empty; //data koncertu

            public string Venue { get; set; } = "Venue unknown"; //lokalizacja koncertu
        }

        /// <summary>
        /// Reprezentuje datę wydarzenia.
        /// </summary>
        public class EventDate //start wydarzenia
        {
            [JsonProperty("start")]
            public EventStart Start { get; set; } = new EventStart();
        }

        /// <summary>
        /// Reprezentuje szczegóły rozpoczęcia wydarzenia.
        /// </summary>
        public class EventStart //data rozpoczecia
        {
            [JsonProperty("localDate")]
            public string LocalDate { get; set; } = string.Empty;
        }

        /// <summary>
        /// Reprezentuje osadzone informacje w odpowiedzi z API, takie jak informacje o miejscach.
        /// </summary>
        public class EventEmbedded //lista miejsc
        {
            [JsonProperty("venues")]
            public List<Venue> Venues { get; set; } = new List<Venue>();
        }

        /// <summary>
        /// Reprezentuje miejsce, w którym odbywa się wydarzenie.
        /// </summary>
        public class Venue //lokalizacja wydarzenia
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;
        }

    }

}
